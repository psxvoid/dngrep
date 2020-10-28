using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dngrep.core.CompilationExtensions;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using Microsoft.Build.Locator;

namespace dngrep.tool.Core
{
    public interface IProjectGrep
    {
        Task FolderAsync(GrepOptions options);
    }

    public class Grep : IProjectGrep
    {
        private readonly IMSBuildWorkspaceStatic workspaceStatic;

        public Grep(IMSBuildWorkspaceStatic workspaceStatic)
        {
            this.workspaceStatic = workspaceStatic;
        }

        public Grep() : this(new MSBuildWorkspaceStatic())
        {
        }

        public async Task FolderAsync(GrepOptions options)
        {
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
                    IReadOnlyCollection<string>? projectMethods = cSharpCompilation
                        .MSCSharpCompilation
                        .GetMethodNames();

                    methodNames.AddRange(projectMethods);
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

