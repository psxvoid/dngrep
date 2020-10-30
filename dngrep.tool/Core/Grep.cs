using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using dngrep.tool.Core.Output.Presenters;
using Microsoft.Build.Locator;
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
        private readonly ISyntaxNodePresenter presenter;

        public Grep(
            IMSBuildWorkspaceStatic workspaceStatic,
            ISyntaxNodePresenter presenter
            )
        {
            this.workspaceStatic = workspaceStatic;
            this.presenter = presenter;
        }

        public async Task FolderAsync(GrepOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            string? currentDirectory = Directory.GetCurrentDirectory();

            (SolutionAndProjectExplorer.PathKind kind, string path) =
                SolutionAndProjectExplorer.GetSolutionOrProject(currentDirectory);

            MSBuildLocator.RegisterDefaults();

            using IMSBuildWorkspace? workspace = this.workspaceStatic.Create();
            workspace.LoadMetadataForReferencedProjects = true;
            // workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

            IEnumerable<IProject> projects = await workspace.GetProjectsAsync(kind, path)
                .ConfigureAwait(false);

            foreach (var proj in projects)
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
                            null);
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
