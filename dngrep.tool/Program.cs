using dngrep.core.CompilationExtensions;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace dngrep.tool
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Should be fixed during further development.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Should be fixed during further development.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Should be fixed during further development.")]
        static async Task Main(string[] args)
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
