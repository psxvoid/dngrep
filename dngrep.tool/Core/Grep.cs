using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
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

        public Grep(
            IMSBuildWorkspaceStatic workspaceStatic
            )
        {
            this.workspaceStatic = workspaceStatic;
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

            var methodNames = new List<string>();

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
                            options.TargetName,
                            null);
                        var query = SyntaxTreeQueryBuilder.From(queryDescriptor);
                        var walker = new SyntaxTreeQueryWalker(query);
                        walker.Visit(tree.GetRoot());
                        methodNames.AddRange(walker.Results
                            .Select(x => x.TryGetIdentifierName() ?? string.Empty)
                            .Where(x => !string.IsNullOrWhiteSpace(x)));
                    }
                }
                else
                {
                    throw new InvalidOperationException("Only C# projects are supported.");
                }
            }

            foreach (var methodName in methodNames)
            {
                System.Console.WriteLine(methodName);
            }
        }
    }
}

