using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.Core.Exceptions;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using dngrep.tool.Core.Output.Presenters;
using Microsoft.CodeAnalysis;

namespace dngrep.tool.Core
{
    public interface IProjectGrep
    {
        Task FolderAsync(GrepOptions options);
        Task TextAsSyntaxTree(GrepOptions options, string code);
    }

    public class Grep : IProjectGrep
    {
        private readonly IMSBuildWorkspaceStatic workspaceStatic;
        private readonly ISolutionAndProjectExplorer explorer;
        private readonly IWorkspaceProjectReader workspaceProjectReader;
        private readonly ICSharpSyntaxTreeStatic syntaxTreeStatic;
        private readonly IPresenterFactory presenterFactory;
        private readonly IFileSystem fs;

        public Grep(
            IMSBuildWorkspaceStatic workspaceStatic,
            ISolutionAndProjectExplorer explorer,
            IPresenterFactory presenterFactory,
            IFileSystem fs,
            IWorkspaceProjectReader projectReader,
            ICSharpSyntaxTreeStatic syntaxTreeStatic)
        {
            this.workspaceStatic = workspaceStatic;
            this.explorer = explorer;
            this.presenterFactory = presenterFactory;
            this.fs = fs;
            this.workspaceProjectReader = projectReader;
            this.syntaxTreeStatic = syntaxTreeStatic;
        }

        public async Task FolderAsync(GrepOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            string? currentDirectory;

            try
            {
                currentDirectory = this.fs.Directory.GetCurrentDirectory();
            }
            catch (UnauthorizedAccessException accessException)
            {
                throw new GrepException(
                    "The application doesn't have required permissions to the current directory.",
                    accessException);
            }
            catch (NotSupportedException notSupportedException)
            {
                throw new GrepException(
                    "The application doesn't support Windows CE.",
                    notSupportedException);
            }
            catch (Exception ex)
            {
                throw new GrepException(
                    "The application is unable to get a path of the current directory.", ex);
            }

            if (string.IsNullOrWhiteSpace(currentDirectory))
            {
                throw new GrepException(
                    "The application retrieved a path of the current directory but it's empty.");
            }

            (SolutionAndProjectExplorer.PathKind kind, string path) =
                this.explorer.GetSolutionOrProject(currentDirectory);

            using IMSBuildWorkspace? workspace = this.workspaceStatic.Create();
            workspace.LoadMetadataForReferencedProjects = true;
            // workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

            IEnumerable<IProject> projects = Enumerable.Empty<IProject>();
            try
            {
                projects = await this.workspaceProjectReader
                    .GetProjectsAsync(workspace, kind, path)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new GrepException(
                    "The application is unable to read one or more solutions or projects.", ex);
            }

            bool hasAnyProjects = false;
            bool hasNonCSharpProjects = false;
            bool hasCSharpProjects = false;
            bool hasNonCompilableProjects = false;
            bool hasAnySearchableUnits = false;
            bool hasAnyResults = false;

            ISyntaxNodePresenter presenter = this.presenterFactory.GetPresenter(
                        options.OutputType ?? PresenterKind.Search);

            foreach (var proj in projects ?? Enumerable.Empty<IProject>())
            {
                hasAnyProjects = true;

                ICompilation? compilation = await proj.GetCompilationAsync().ConfigureAwait(false);

                if (compilation != null && compilation is ICSharpCompilation cSharpCompilation)
                {
                    hasCSharpProjects = true;

                    foreach (
                        SyntaxTree? tree
                        in compilation.MSCompilation?.SyntaxTrees
                        ?? Enumerable.Empty<SyntaxTree>()
                        )
                    {
                        hasAnySearchableUnits = true;

                        IReadOnlyCollection<SyntaxNode>? results = QueryNodes(options, tree);

                        if (results.Count > 0)
                        {
                            hasAnyResults = true;
                            presenter.ProduceOutput(results, options);
                        }
                    }
                }
                else if (compilation != null)
                {
                    hasNonCSharpProjects = true;
                }
                else
                {
                    hasNonCompilableProjects = true;
                }
            }

            presenter.Flush();

            if (!hasAnyProjects)
            {
                throw new GrepException(
                    "The application was unable to find any projects." +
                    " Please ensure that the current folder" +
                    " contains a solution or a project.");
            }

            if (hasNonCSharpProjects && !hasCSharpProjects)
            {
                throw new GrepException(
                    "The application found at least one project but it's not a C# project.");
            }

            if (hasNonCompilableProjects && !hasCSharpProjects)
            {
                throw new GrepException(
                    "The application found at least one project but it is not compilable.");
            }

            // situations where C# projects exists with non-C# or non-compilable project
            // are normal and shouldn't be handled separately

            if (!hasAnySearchableUnits)
            {
                throw new GrepException(
                    "At lease one project was detected and compiled but it doesn't have any code.");
            }

            if (!hasAnyResults)
            {
                throw new GrepException(
                    "At least one C# project detected and compiled but nothing is found.");
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task TextAsSyntaxTree(GrepOptions options, string sourceCode)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            _ = sourceCode ?? throw new ArgumentNullException(nameof(sourceCode));

            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                throw new GrepException("Unable to run the query on the empty text.");
            }

            SyntaxTree tree = this.syntaxTreeStatic.ParseText(sourceCode);
            ISyntaxNodePresenter presenter =
                this.presenterFactory.GetPresenter(options.OutputType ?? PresenterKind.Search);

            IReadOnlyCollection<SyntaxNode>? results = QueryNodes(options, tree);

            if (results.Count > 0)
            {
                // hasAnyResults = true;
                presenter.ProduceOutput(results, options);
                presenter.Flush();
            }
        }

        private static IReadOnlyCollection<SyntaxNode> QueryNodes(GrepOptions options, SyntaxTree tree)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            var queryDescriptor = new SyntaxTreeQueryDescriptor
            {
                Target = options.Target ?? QueryTarget.Any,
                Scope = options.Scope ?? QueryTargetScope.None,
                AccessModifier = QueryAccessModifier.Any,
                TargetNameContains = options.Contains ?? Enumerable.Empty<string>(),
                TargetNameExcludes = options.Exclude ?? Enumerable.Empty<string>(),
                TargetScopeContains = options.ScopeContains ?? Enumerable.Empty<string>(),
                TargetScopeExcludes = options.ScopeExclude ?? Enumerable.Empty<string>(),
                TargetPathContains = options.PathContains ?? Enumerable.Empty<string>(),
                TargetPathExcludes = options.PathExclude ?? Enumerable.Empty<string>(),
                EnableRegex = options.EnableRegexp ?? false
            };

            SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);
            var walker = new SyntaxTreeQueryWalker(query);
            walker.Visit(tree.GetRoot());

            return walker.Results;
        }
    }
}
