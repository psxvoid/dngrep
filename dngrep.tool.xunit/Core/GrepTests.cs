using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.Core.Exceptions;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using dngrep.tool.xunit.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static dngrep.tool.Core.FileSystem.SolutionAndProjectExplorer;

namespace dngrep.tool.xunit.Core
{
    public class GrepTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IDirectory> directoryMock;
        private readonly Mock<ISolutionAndProjectExplorer> explorerMock;
        private readonly Mock<IWorkspaceProjectReader> projectReaderMock;
        private readonly Mock<IMSBuildWorkspace> workspaceMock;
        private readonly Mock<IMSBuildWorkspaceStatic> workspaceStaticMock;
        private readonly Grep sut;

        public GrepTests()
        {
            this.fixture = AutoFixtureFactory.Default();

            this.directoryMock = this.fixture.Freeze<Mock<IDirectory>>();
            var fsMock = this.fixture.Freeze<Mock<IFileSystem>>();
            fsMock
                .SetupGet(x => x.Directory)
                .Returns(this.directoryMock.Object);

            this.explorerMock = this.fixture.Freeze<Mock<ISolutionAndProjectExplorer>>();
            this.workspaceMock = this.fixture.Freeze<Mock<IMSBuildWorkspace>>();
            this.workspaceStaticMock = this.fixture.Freeze<Mock<IMSBuildWorkspaceStatic>>();
            this.workspaceStaticMock
                .Setup(x => x.Create())
                .Returns(this.workspaceMock.Object);

            this.projectReaderMock = this.fixture.Freeze<Mock<IWorkspaceProjectReader>>();

            this.sut = this.fixture.Create<Grep>();
        }

        [Fact]
        public async Task FolderAsync_Null_ShouldThrow()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.sut.FolderAsync(null))
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_NullDirectoryDefaultOptions_ShouldThrow()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            this.WithCurrentDirectory(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<GrepException>(
                () => this.sut.FolderAsync(options)).ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_EmptyDirectory_ShouldThrow()
        {
            this.WithCurrentDirectory("\t   ");
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<GrepException>(
                () => this.sut.FolderAsync(options)).ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_DirectoryAccessException_ShouldThrow()
        {
            this.directoryMock.Setup(x => x.GetCurrentDirectory())
                .Throws<UnauthorizedAccessException>();
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<GrepException>(
                () => this.sut.FolderAsync(options)).ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_DirectoryNotSupportedException_ShouldThrow()
        {
            this.directoryMock.Setup(x => x.GetCurrentDirectory())
                .Throws<NotSupportedException>();
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<GrepException>(
                () => this.sut.FolderAsync(options)).ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_DirectoryException_ShouldThrow()
        {
            this.directoryMock.Setup(x => x.GetCurrentDirectory())
                .Throws<Exception>();
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<GrepException>(
                () => this.sut.FolderAsync(options)).ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_NonEmptyDirAndProjectsThrow_ShouldThrow()
        {
            this.WithCurrentDirectory("x:/test.sln");
            this.WithProjectsThrows<Exception>();
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await Assert.ThrowsAsync<GrepException>(() => this.sut.FolderAsync(options))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task FolderAsync_NonEmptyDirAndNullProjects_ShouldGetProjectOrSolution()
        {
            this.WithCurrentDirectory("x:/test.sln");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            this.WithProjects(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await this.sut.FolderAsync(options).ConfigureAwait(false);

            this.explorerMock.Verify(x => x.GetSolutionOrProject(
                It.Is<string>(it => it == "x:/test.sln")), Times.Once());
        }

        [Fact]
        public async Task FolderAsync_NonEmptyDirAndEmptyProjects_ShouldGetProjectOrSolution()
        {
            this.WithCurrentDirectory("x:/test.sln");
            this.WithProjects(Enumerable.Empty<IProject>());
            GrepOptions options = this.fixture.Build<GrepOptions>()
                .WithAutoProperties()
                .Create();

            await this.sut.FolderAsync(options).ConfigureAwait(false);

            this.explorerMock.Verify(x => x.GetSolutionOrProject(
                It.Is<string>(it => it == "x:/test.sln")), Times.Once());
        }

        private void WithCurrentDirectory(string path) =>
            this.directoryMock
                .Setup(x => x.GetCurrentDirectory())
                .Returns(path);

        private void WithProjects(IEnumerable<IProject> projects) =>
            this.projectReaderMock
                .Setup(x => x.GetProjectsAsync(
                    It.IsAny<IMSBuildWorkspace>(),
                    It.IsAny<PathKind>(),
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(Task.FromResult(projects));

        private void WithProjectsThrows<T>() where T : Exception, new() =>
            this.projectReaderMock
                .Setup(x => x.GetProjectsAsync(
                    It.IsAny<IMSBuildWorkspace>(),
                    It.IsAny<PathKind>(),
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Throws<T>();
    }
}
