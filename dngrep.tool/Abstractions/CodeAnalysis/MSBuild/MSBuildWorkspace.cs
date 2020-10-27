using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.MSBuild;

namespace dngrep.tool.Abstractions.CodeAnalysis.MSBuild
{
    public interface IMSBuildWorkspace : IDisposable
    {
        bool LoadMetadataForReferencedProjects { get; set; }
        Task<ISolution> OpenSolutionAsync(string solutionFilePath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);
        Task<ISolution> OpenSolutionAsync(string solutionFilePath, ILogger msbuildLogger, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);
        Task<IProject> OpenProjectAsync(string projectFilePath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);
        Task<IProject> OpenProjectAsync(string projectFilePath, ILogger msbuildLogger, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default);
    }

    public interface IMSBuildWorkspaceStatic
    {
        /// <summary>
        /// This is an abstraction for <see cref="MSBuildWorkspace.Create"/>
        /// </summary>
        /// <returns>An instance of <see cref="MSBuildWorkspace"/></returns>
        IMSBuildWorkspace Create();
    }

    public sealed class MSBuildWorkspaceDecorator : IMSBuildWorkspace, IDisposable
    {
        public MSBuildWorkspaceDecorator(MSBuildWorkspace workspace)
        {
            this.Workspace = workspace;
        }

        public MSBuildWorkspace Workspace { get; }

        bool IMSBuildWorkspace.LoadMetadataForReferencedProjects
        {
            get => this.Workspace.LoadMetadataForReferencedProjects;
            set => this.Workspace.LoadMetadataForReferencedProjects = value;
        }

        public void Dispose()
        {
            this.Workspace.Dispose();
        }

        public async Task<IProject> OpenProjectAsync(string projectFilePath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            return new Project(await this.Workspace.OpenProjectAsync(projectFilePath, progress, cancellationToken).ConfigureAwait(false));
        }

        public async Task<IProject> OpenProjectAsync(string projectFilePath, ILogger msbuildLogger, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            return new Project(await this.Workspace.OpenProjectAsync(projectFilePath, msbuildLogger, progress, cancellationToken).ConfigureAwait(false));
        }

        public async Task<ISolution> OpenSolutionAsync(string solutionFilePath, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            return new Solution(await this.Workspace.OpenSolutionAsync(solutionFilePath, progress, cancellationToken).ConfigureAwait(false));
        }

        public async Task<ISolution> OpenSolutionAsync(string solutionFilePath, ILogger msbuildLogger, IProgress<ProjectLoadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            return new Solution(await this.Workspace.OpenSolutionAsync(solutionFilePath, msbuildLogger, progress, cancellationToken).ConfigureAwait(false));
        }
    }

    public class MSBuildWorkspaceStatic : IMSBuildWorkspaceStatic
    {
        public IMSBuildWorkspace Create() =>
            new MSBuildWorkspaceDecorator(MSBuildWorkspace.Create());

    }
}
