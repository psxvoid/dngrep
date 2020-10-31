using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
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
    }

    public class Grep : IProjectGrep
    {
        private readonly IMSBuildWorkspaceStatic workspaceStatic;
        private readonly ISolutionAndProjectExplorer explorer;
        private readonly IWorkspaceProjectReader workspaceProjectReader;
        private readonly ISyntaxNodePresenter presenter;
        private readonly IFileSystem fs;

        public Grep(
            IMSBuildWorkspaceStatic workspaceStatic,
            ISolutionAndProjectExplorer explorer,
            ISyntaxNodePresenter presenter,
            IFileSystem fs,
            IWorkspaceProjectReader projectReader)
        {
            this.workspaceStatic = workspaceStatic;
            this.explorer = explorer;
            this.presenter = presenter;
            this.fs = fs;
            this.workspaceProjectReader = projectReader;
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
                    "The application is unable to read one or more solution or project.", ex);
            }

            foreach (var proj in projects ?? Enumerable.Empty<IProject>())
            {
                ICompilation? compilation = await proj.GetCompilationAsync().ConfigureAwait(false);

                if (compilation != null && compilation is ICSharpCompilation cSharpCompilation)
                {
                    foreach (SyntaxTree? tree in compilation.MSCompilation.SyntaxTrees)
                    {
                        var queryDescriptor = new SyntaxTreeQueryDescriptor(
                            options.Target ?? QueryTarget.Any,
                            QueryAccessModifier.Any,
                            options.Scope ?? QueryTargetScope.None,
                            options.Contains,
                            options.Exclude,
                            null,
                            options.EnableRegexp ?? false);
                        var query = SyntaxTreeQueryBuilder.From(queryDescriptor);
                        var walker = new SyntaxTreeQueryWalker(query);
                        walker.Visit(tree.GetRoot());

                        this.presenter.ProduceOutput(walker.Results, options);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Only C# projects are supported.");
                }
            }
        }
    }
}
