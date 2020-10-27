using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dngrep.core.CompilationExtensions;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

namespace dngrep.tool.Core
{
    public static class Grep
    {
        public static async Task FolderAsync(GrepOptions options)
        {
            string? currentDirectory = Directory.GetCurrentDirectory();

            (SolutionAndProjectExplorer.PathKind kind, string path) =
                SolutionAndProjectExplorer.GetSolutionOrProject(currentDirectory);

            MSBuildLocator.RegisterDefaults();

            using var workspace = MSBuildWorkspace.Create();
            workspace.LoadMetadataForReferencedProjects = true;
            // workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

            IEnumerable<Project> projects;

            switch (kind)
            {
                case SolutionAndProjectExplorer.PathKind.None:
                    Console.WriteLine("No solution or project is found in the current directory");
                    return;
                case SolutionAndProjectExplorer.PathKind.Solution:
                    {
                        Solution solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
                        projects = solution.Projects;
                    }
                    break;
                case SolutionAndProjectExplorer.PathKind.Project:
                    {
                        Project project = await workspace.OpenProjectAsync(path).ConfigureAwait(false);
                        projects = new[] { project };
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unsupported path target.");
            }

            var methodNames = new List<string>();

            foreach (var proj in projects)
            {
                Compilation? compilation = await proj.GetCompilationAsync().ConfigureAwait(false);

                if (compilation != null && compilation is CSharpCompilation cSharpCompilation)
                {
                    IReadOnlyCollection<string>? projectMethods = cSharpCompilation.GetMethodNames();

                    methodNames.AddRange(projectMethods);
                }
                else
                {
                    throw new InvalidOperationException("Only C# projects are supported.");
                }
            }

            foreach (var methodName in methodNames)
            {
                Console.WriteLine(methodName);
            }
        }
    }
}

