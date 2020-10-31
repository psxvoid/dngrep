using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using static dngrep.tool.Core.FileSystem.SolutionAndProjectExplorer;

namespace dngrep.tool.Core.CodeAnalysis.MSBuild
{
    public interface IWorkspaceProjectReader
    {
        Task<IEnumerable<IProject>> GetProjectsAsync(IMSBuildWorkspace workspace, PathKind kind, string path, ILogger? logger = null);
    }

    public class WorkspaceProjectReader : IWorkspaceProjectReader
    {
        public async Task<IEnumerable<IProject>> GetProjectsAsync(IMSBuildWorkspace workspace, PathKind kind, string path, ILogger? logger = null)
        {
            _ = workspace ?? throw new ArgumentNullException(nameof(workspace));

            if (kind != PathKind.None && path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (kind != PathKind.None && string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path to the project or solution cannot be empty");
            }

            IEnumerable<IProject> projects = Enumerable.Empty<IProject>();

            switch (kind)
            {
                case PathKind.None:
                    logger?.LogWarning("No solution or project is found in the current directory");
                    break;

                case PathKind.Solution:
                    {
                        ISolution solution = await workspace.OpenSolutionAsync(path)
                            .ConfigureAwait(false);

                        projects = solution.Projects;
                    }
                    break;

                case PathKind.Project:
                    {
                        IProject project = await workspace.OpenProjectAsync(path)
                            .ConfigureAwait(false);

                        projects = new[] { project };
                    }
                    break;

                default:
                    throw new InvalidOperationException("Unsupported path target.");
            }

            return projects;
        }
    }
}
