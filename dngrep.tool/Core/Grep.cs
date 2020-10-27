﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dngrep.core.CompilationExtensions;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using Microsoft.Build.Locator;

namespace dngrep.tool.Core
{
    public class Grep
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

            IEnumerable<IProject> projects;

            switch (kind)
            {
                case SolutionAndProjectExplorer.PathKind.None:
                    Console.WriteLine("No solution or project is found in the current directory");
                    return;
                case SolutionAndProjectExplorer.PathKind.Solution:
                    {
                        ISolution solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
                        projects = solution.Projects;
                    }
                    break;
                case SolutionAndProjectExplorer.PathKind.Project:
                    {
                        IProject project = await workspace.OpenProjectAsync(path).ConfigureAwait(false);
                        projects = new[] { project };
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unsupported path target.");
            }

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
                Console.WriteLine(methodName);
            }
        }
    }
}

