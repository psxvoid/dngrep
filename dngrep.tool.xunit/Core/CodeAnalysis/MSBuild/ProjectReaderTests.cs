using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.xunit.TestHelpers;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static dngrep.tool.Core.FileSystem.SolutionAndProjectExplorer;

namespace dngrep.tool.xunit.Core.CodeAnalysis.MSBuild
{
    public static class ProjectReaderTests
    {
        public class NoneKind
        {
            private readonly IFixture fixture;
            private readonly Mock<ILogger> loggerMock;

            private readonly Mock<IMSBuildWorkspace> workspaceMock;

            public NoneKind()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.loggerMock = this.fixture.Freeze<Mock<ILogger>>();
                this.workspaceMock = new Mock<IMSBuildWorkspace>();
            }

            [Fact]
            public async Task GetProjectsAsync_NoneKindAnyPathLoggerSpecified_ShouldLogWarning()
            {
                await this.workspaceMock.Object.GetProjectsAsync(
                     PathKind.None,
                     "any",
                     this.loggerMock.Object).ConfigureAwait(false);

                this.loggerMock.VerifyLogLevel(LogLevel.Warning, Times.Once());
            }

            [Fact]
            public async Task GetProjectsAsync_NoneKindEmptyPathLoggerSpecified_ShouldLogWarning()
            {
                await this.workspaceMock.Object.GetProjectsAsync(
                     PathKind.None,
                     "  ",
                     this.loggerMock.Object).ConfigureAwait(false);

                this.loggerMock.VerifyLogLevel(LogLevel.Warning, Times.Once());
            }

            [Fact]
            public async Task GetProjectsAsync_NoneKindEmptyPathLoggerSpecified_ShouldReturnEmpty()
            {
                Assert.Empty(await this.workspaceMock.Object.GetProjectsAsync(
                     PathKind.None,
                     "  ",
                     this.loggerMock.Object).ConfigureAwait(false));
            }
        }

        public class SolutionKind
        {
            private readonly IFixture fixture;
            private readonly Mock<ILogger> loggerMock;
            private readonly IEnumerable<IProject> projects;
            private readonly Mock<ISolution> solutionMock;

            private readonly Mock<IMSBuildWorkspace> workspaceMock;

            public SolutionKind()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.loggerMock = this.fixture.Freeze<Mock<ILogger>>();
                this.projects = this.fixture.CreateMany<IProject>();
                this.solutionMock = new Mock<ISolution>();
                this.solutionMock.SetupGet(x => x.Projects)
                    .Returns(this.projects);

                this.workspaceMock = new Mock<IMSBuildWorkspace>();
                this.workspaceMock.Setup(x => x.OpenSolutionAsync(
                    It.IsAny<string>(),
                    It.IsAny<IProgress<ProjectLoadProgress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(this.solutionMock.Object));
            }

            [Fact]
            public async Task GetProjectsAsync_SolutionKindNullPath_ShouldThrow()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await this.workspaceMock.Object.GetProjectsAsync(
                        PathKind.Solution,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                        this.loggerMock.Object).ConfigureAwait(false)).ConfigureAwait(false);
            }

            [Fact]
            public async Task GetProjectsAsync_SolutionKindEmptyPath_ShouldThrow()
            {
                await Assert.ThrowsAsync<ArgumentException>(
                    async () => await this.workspaceMock.Object.GetProjectsAsync(
                        PathKind.Solution,
                        "   ",
                        this.loggerMock.Object).ConfigureAwait(false)).ConfigureAwait(false);
            }

            [Fact]
            public async Task GetProjectsAsync_SolutionKindNonEmptyPath_ShouldPassCorrectPathToWorkspace()
            {
                IEnumerable<IProject> actual = await this.workspaceMock.Object.GetProjectsAsync(
                    PathKind.Solution,
                    "x:/solution.sln",
                    this.loggerMock.Object).ConfigureAwait(false);

                this.workspaceMock.Verify(x => x.OpenSolutionAsync(
                    It.Is<string>(it => it == "x:/solution.sln"),
                    It.IsAny<IProgress<ProjectLoadProgress>>(),
                    It.IsAny<CancellationToken>()), Times.Once());
            }

            [Fact]
            public async Task GetProjectsAsync_SolutionKindNonEmptyPath_ShouldReturnProjectsFromSolution()
            {
                IEnumerable<IProject> actual = await this.workspaceMock.Object.GetProjectsAsync(
                     PathKind.Solution,
                     "x:/solution.sln",
                     this.loggerMock.Object).ConfigureAwait(false);

                Assert.Same(this.projects, actual);
            }
        }

        public class ProjectKind
        {
            private readonly IFixture fixture;
            private readonly Mock<ILogger> loggerMock;
            private readonly Mock<IProject> project;

            private readonly Mock<IMSBuildWorkspace> workspaceMock;

            public ProjectKind()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.loggerMock = this.fixture.Freeze<Mock<ILogger>>();
                this.project = new Mock<IProject>();

                this.workspaceMock = new Mock<IMSBuildWorkspace>();
                this.workspaceMock.Setup(x => x.OpenProjectAsync(
                    It.IsAny<string>(),
                    It.IsAny<IProgress<ProjectLoadProgress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(this.project.Object));
            }

            [Fact]
            public async Task GetProjectsAsync_ProjectKindNullPath_ShouldThrow()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(
                    async () => await this.workspaceMock.Object.GetProjectsAsync(
                        PathKind.Project,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                        this.loggerMock.Object).ConfigureAwait(false)).ConfigureAwait(false);
            }

            [Fact]
            public async Task GetProjectsAsync_ProjectKindWhitespacePath_ShouldThrow()
            {
                await Assert.ThrowsAsync<ArgumentException>(
                    async () => await this.workspaceMock.Object.GetProjectsAsync(
                        PathKind.Project,
                        "   ",
                        this.loggerMock.Object).ConfigureAwait(false)).ConfigureAwait(false);
            }

            [Fact]
            public async Task GetProjectsAsync_ProjectKindNonEmptyPath_ShouldPassCorrectPathToWorkspace()
            {
                IEnumerable<IProject> actual = await this.workspaceMock.Object.GetProjectsAsync(
                     PathKind.Project,
                     "x:/proj.csproj",
                     this.loggerMock.Object).ConfigureAwait(false);

                this.workspaceMock.Verify(x => x.OpenProjectAsync(
                    It.Is<string>(it => it == "x:/proj.csproj"),
                    It.IsAny<IProgress<ProjectLoadProgress>>(),
                    It.IsAny<CancellationToken>()), Times.Once());
            }

            [Fact]
            public async Task GetProjectsAsync_ProjectKindNonEmptyPath_ShouldReturnOpenedProject()
            {
                IEnumerable<IProject> actual = await this.workspaceMock.Object.GetProjectsAsync(
                     PathKind.Project,
                     "x:/proj.csproj",
                     this.loggerMock.Object).ConfigureAwait(false);

                Assert.Same(this.project.Object, actual.First());
            }
        }
    }
}
