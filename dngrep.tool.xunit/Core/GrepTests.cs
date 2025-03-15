using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries.Specifiers;
using dngrep.tool.Abstractions.CodeAnalysis;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using dngrep.tool.Abstractions.CodeAnalysis.MSBuild;
using dngrep.tool.Core;
using dngrep.tool.Core.CodeAnalysis.MSBuild;
using dngrep.tool.Core.Exceptions;
using dngrep.tool.Core.FileSystem;
using dngrep.tool.Core.Options;
using dngrep.tool.Core.Output.Presenters;
using dngrep.tool.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static dngrep.tool.Core.FileSystem.SolutionAndProjectExplorer;

namespace dngrep.tool.xunit.Core
{
    public static class GrepTests
    {
        public class FolderAsync
        {
            private readonly IFixture fixture;
            private readonly Mock<IDirectory> directoryMock;
            private readonly Mock<IWorkspaceProjectReader> projectReaderMock;
            private readonly Mock<ISyntaxNodePresenter> presenterMock;
            private readonly Mock<IPresenterFactory> presenterFactoryMock;
            private readonly Mock<IMSBuildWorkspace> workspaceMock;
            private readonly Mock<IMSBuildWorkspaceStatic> workspaceStaticMock;
            private readonly Grep sut;

            public FolderAsync()
            {
                this.fixture = AutoFixtureFactory.Default();

                this.directoryMock = this.fixture.Freeze<Mock<IDirectory>>();
                Mock<IFileSystem> fsMock = this.fixture.Freeze<Mock<IFileSystem>>();
                fsMock
                    .SetupGet(x => x.Directory)
                    .Returns(this.directoryMock.Object);

                this.fixture.Freeze<Mock<ISolutionAndProjectExplorer>>();
                this.workspaceMock = this.fixture.Freeze<Mock<IMSBuildWorkspace>>();
                this.workspaceStaticMock = this.fixture.Freeze<Mock<IMSBuildWorkspaceStatic>>();
                this.workspaceStaticMock
                    .Setup(x => x.Create())
                    .Returns(this.workspaceMock.Object);

                this.projectReaderMock = this.fixture.Freeze<Mock<IWorkspaceProjectReader>>();
                this.presenterMock = this.fixture.Freeze<Mock<ISyntaxNodePresenter>>();
                this.presenterFactoryMock = this.fixture.Freeze<Mock<IPresenterFactory>>();
                this.presenterFactoryMock
                    .Setup(x => x.GetPresenter(It.IsAny<PresenterKind>()))
                    .Returns(this.presenterMock.Object);

                this.sut = this.fixture.Create<Grep>();
            }

            [Fact]
            public async Task Null_ShouldThrow()
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await Assert.ThrowsAsync<ArgumentNullException>(() => this.sut.FolderAsync(null))
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                .ConfigureAwait(false);
            }

            [Fact]
            public async Task NullDirectoryDefaultOptions_ShouldThrow()
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
            public async Task EmptyDirectory_ShouldThrow()
            {
                this.WithCurrentDirectory("\t   ");
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .WithAutoProperties()
                    .Create();

                await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);
            }

            [Fact]
            public async Task DirectoryAccessException_ShouldThrow()
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
            public async Task DirectoryNotSupportedException_ShouldThrow()
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
            public async Task DirectoryException_ShouldThrow()
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
            public async Task NonEmptyDirAndProjectsThrow_ShouldThrow()
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
            public async Task NonEmptyDirAndNullProjects_ShouldThrow()
            {
                this.WithCurrentDirectory("x:/test.sln");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                this.WithProjects(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .WithAutoProperties()
                    .Create();

                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);

                Assert.Equal(
                    "The application was unable to find any projects." +
                    " Please ensure that the current folder" +
                    " contains a solution or a project.",
                    exception.Message);
            }

            [Fact]
            public async Task NonEmptyDirAndEmptyProjects_ShouldThrow()
            {
                this.WithCurrentDirectory("x:/test.sln");
                this.WithProjects(Enumerable.Empty<IProject>());
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .WithAutoProperties()
                    .Create();

                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);

                Assert.Equal(
                    "The application was unable to find any projects." +
                    " Please ensure that the current folder" +
                    " contains a solution or a project.",
                    exception.Message);
            }

            [Fact]
            public async Task NonEmptyDirNonCSharpProject_ShouldThrow()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult(this.fixture.Create<ICompilation?>()));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .WithAutoProperties()
                    .Create();

                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);

                Assert.Equal(
                    "The application found at least one project but it's not a C# project.",
                    exception.Message);
            }

            [Fact]
            public async Task NonEmptyDirSingleProjectNotCSharpCompilation_ShouldThrow()
            {
                this.WithCurrentDirectory("x:/test.sln");
                ICompilation? compilationWrapper = CreateCompilation(null);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .WithAutoProperties()
                    .Create();

                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);

                Assert.Equal(
                    "The application found at least one project but it's not a C# project.",
                    exception.Message);
            }

            [Fact]
            public async Task NonEmptyDirCSharpProjectCSharpCompilationNoCode_ShouldThrow()
            {
                this.WithCurrentDirectory("x:/test.sln");

                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(null);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .WithAutoProperties()
                    .Create();

                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);

                Assert.Equal(
                     "At lease one project was detected and compiled but it doesn't have any code.",
                    exception.Message);
            }

            [Fact]
            public async Task NoErrorsButNothingFound_ShouldThrow()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System;");

                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .Create();

                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.FolderAsync(options)).ConfigureAwait(false);

                Assert.Equal(
                    "At least one C# project detected and compiled but nothing is found.",
                    exception.Message);
            }

            [Fact]
            public async Task NoErrorsAndHasMatches_ShouldGetDefaultPresenter()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System; class Test {};");
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterFactoryMock.Verify(
                    x => x.GetPresenter(It.Is<PresenterKind>(it => it == PresenterKind.Search)),
                    Times.Once());
            }

            [Fact]
            public async Task NoErrorsAndHasMatchesAndPresenter_ShouldGetSpecificPresenter()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System; class Test {};");
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .With(x => x.OutputType, PresenterKind.Statistics)
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterFactoryMock.Verify(
                    x => x.GetPresenter(It.Is<PresenterKind>(it => it == PresenterKind.Statistics)),
                    Times.Once());
            }

            [Fact]
            public async Task NoErrorsAndHasMatchesAndPresenter_ShouldFlushPresenterOnce()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System; class Test {};");
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .With(x => x.OutputType, PresenterKind.Statistics)
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterMock.Verify(x => x.Flush(), Times.Once());
            }

            [Fact]
            public async Task NoErrorsAndHasMatches_ShouldPresentResults()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System; class Test {};");
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterMock.Verify(x => x.ProduceOutput(
                    It.Is<IEnumerable<SyntaxNode>>(
                        it => it.Count() == 1 && it.Any(x => x.TryGetIdentifierName() == "Test")),
                    It.Is<GrepOptions>(it => it == options)), Times.Once());
            }

            [Fact]
            public async Task NoErrorsAndNamedScope_ShouldFilterByScopeName()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System;
                    class Test1 { void MyMethodX() {} }
                    class Test2 { void MyMethodY() {} }
                    ");
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .With(x => x.Target, QueryTarget.Method)
                    .With(x => x.Scope, QueryTargetScope.Class)
                    .With(x => x.ScopeContains, new[] { "st1" })
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterMock.Verify(x => x.ProduceOutput(
                    It.Is<IEnumerable<SyntaxNode>>(
                        it => it.Count() == 1 && it.Any(x => x.TryGetIdentifierName() == "MyMethodX")),
                    It.Is<GrepOptions>(it => it == options)), Times.Once());
            }

            [Fact]
            public async Task NamedScopeWithExcludeRegexp_ShouldFilterByScopeName()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System;
                    class Test1 { void MyMethodX() {} }
                    class Test2 { void MyMethodY() {} }
                    ");
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .With(x => x.Target, QueryTarget.Method)
                    .With(x => x.Scope, QueryTargetScope.Class)
                    .With(x => x.ScopeContains, new[] { "Test[12]" })
                    .With(x => x.ScopeExclude, new[] { "1" })
                    .With(x => x.EnableRegexp, true)
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterMock.Verify(x => x.ProduceOutput(
                    It.Is<IEnumerable<SyntaxNode>>(
                        it => it.Count() == 1
                        && it.Any(x => x.TryGetIdentifierName() == "MyMethodY")),
                    It.Is<GrepOptions>(it => it == options)), Times.Once());
            }

            [Fact]
            public async Task TargetAndScopeAndPathRegexp_ShouldFilterByScopeNameAndPath()
            {
                this.WithCurrentDirectory("x:/test.sln");
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation =
                    TestCompiler.Compile(@"using System;
                    class Test1 { void MyMethodX() {} }
                    class Test2 { void MyMethodY() {} }
                    ");
                compilation = compilation.AddSyntaxTrees(
                    CSharpSyntaxTree.ParseText(
                        "class Test1 { void MyMethodZ() {} }", path: "x:/test.cs"));
                ICSharpCompilation? compilationWrapper = CreateCSharpCompilation(compilation);
                Mock<IProject> projectMock = this.fixture.Create<Mock<IProject>>();
                projectMock.Setup(x => x.GetCompilationAsync())
                    .Returns(Task.FromResult<ICompilation?>(compilationWrapper));
                this.WithProjects(new[] { projectMock.Object });
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .With(x => x.Target, QueryTarget.Method)
                    .With(x => x.Scope, QueryTargetScope.Class)
                    .With(x => x.ScopeContains, new[] { "Test[12]" })
                    .With(x => x.PathContains, new[] { @"test.cs$" })
                    .With(x => x.EnableRegexp, true)
                    .OmitAutoProperties()
                    .Create();

                await this.sut.FolderAsync(options).ConfigureAwait(false);

                this.presenterMock.Verify(x => x.ProduceOutput(
                    It.IsAny<IEnumerable<SyntaxNode>>(),
                    It.IsAny<GrepOptions>()), Times.Once());
                this.presenterMock.Verify(x => x.ProduceOutput(
                    It.Is<IEnumerable<SyntaxNode>>(
                        it => it.Count() == 1
                        && it.Any(x => x.TryGetIdentifierName() == "MyMethodZ")),
                    It.Is<GrepOptions>(it => it == options)), Times.Once());
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

            private static ICompilation? CreateCompilation(
                Microsoft.CodeAnalysis.Compilation? compilation)
            {
                var compilationMock = new Mock<ICompilation>();
#pragma warning disable CS8604 // Possible null reference argument.
                compilationMock
                    .SetupGet(x => x.MSCompilation)
                    .Returns(compilation);
#pragma warning restore CS8604 // Possible null reference argument.
                return compilationMock.Object;
            }

            private static ICSharpCompilation? CreateCSharpCompilation(
                Microsoft.CodeAnalysis.CSharp.CSharpCompilation? compilation)
            {
                var compilationMock = new Mock<ICSharpCompilation>();
#pragma warning disable CS8604 // Possible null reference argument.
                compilationMock
                    .SetupGet(x => x.MSCompilation)
                    .Returns(compilation);
                compilationMock
                    .SetupGet(x => x.MSCSharpCompilation)
                    .Returns(compilation);
#pragma warning restore CS8604 // Possible null reference argument.
                return compilationMock.Object;
            }
        }

        public class TextAsSyntaxTreeAsync
        {
            private readonly IFixture fixture;
            private readonly Grep sut;

            public TextAsSyntaxTreeAsync()
            {
                this.fixture = AutoFixtureFactory.Default();

                this.fixture.Freeze<Mock<ICSharpSyntaxTreeStatic>>()
                    .Setup(x => x.ParseText(It.IsAny<string>()))
                    .Returns(CSharpSyntaxTree.ParseText(""));
                Mock<ISyntaxNodePresenter> presenterMock =
                    this.fixture.Freeze<Mock<ISyntaxNodePresenter>>();
                this.fixture.Freeze<Mock<IPresenterFactory>>()
                    .Setup(x => x.GetPresenter(It.IsAny<PresenterKind>()))
                    .Returns(presenterMock.Object);

                this.sut = this.fixture.Create<Grep>();
            }

            [Fact]
            public async Task NullOptions_ShouldThrow()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    () => this.sut.TextAsSyntaxTree(null, "any")
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ).ConfigureAwait(false);
            }

            [Fact]
            public async Task NullText_ShouldThrow()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(
                    () => this.sut.TextAsSyntaxTree(
                        new GrepOptions(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ).ConfigureAwait(false);
            }

            [Fact]
            public async Task EmptyOrWhitespaceText_ShouldThrow()
            {
                GrepException exception = await Assert.ThrowsAsync<GrepException>(
                    () => this.sut.TextAsSyntaxTree(new GrepOptions(), "  \t \n")
                ).ConfigureAwait(false);

                Assert.Equal(
                    "Unable to run the query on the empty text.",
                    exception.Message);
            }

            [Fact]
            public async Task DefaultOptionsAndValidCode_ShouldParseTextAsSyntaxTree()
            {
                const string text = "class Person {}";
                Mock<ICSharpSyntaxTreeStatic> parserMock =
                    this.fixture.Create<Mock<ICSharpSyntaxTreeStatic>>();

                await this.sut.TextAsSyntaxTree(new GrepOptions(), text).ConfigureAwait(false);

                parserMock.Verify(x => x.ParseText(It.Is<string>(it => it == text)), Times.Once());
                parserMock.Verify(x => x.ParseText(It.IsAny<string>()), Times.Once());
            }

            [Fact]
            public async Task DefaultOptionsAndParsedClass_ShouldMatchSingleClass()
            {
                const string text = "class Person {}";
                this.WithParsed(text);

                await this.sut.TextAsSyntaxTree(new GrepOptions(), text).ConfigureAwait(false);

                this.VerifyResultContains(x => x.GetType() == typeof(ClassDeclarationSyntax));
            }

            [Fact]
            public async Task DefaultOptionsAndParsedClass_ShouldFlushPresenterOnce()
            {
                const string text = "class Person {}";
                this.WithParsed(text);

                await this.sut.TextAsSyntaxTree(new GrepOptions(), text).ConfigureAwait(false);

                this.fixture.Create<Mock<ISyntaxNodePresenter>>()
                    .Verify(x => x.Flush(), Times.Once());
            }

            [Fact]
            public async Task DefaultOptionsAndParsedTwoClasses_ShouldMatchTwoClasses()
            {
                const string text = "class Person {}; class Student {}";
                this.WithParsed(text);

                await this.sut.TextAsSyntaxTree(new GrepOptions(), text).ConfigureAwait(false);

                this.VerifyResultContains(x => x.GetType() == typeof(ClassDeclarationSyntax), 2);
            }

            [Fact]
            public async Task DefaultOptionsAndParsedTwoClasses_ShouldFlushPresenterOnce()
            {
                const string text = "class Person {}; class Student {}";
                this.WithParsed(text);

                await this.sut.TextAsSyntaxTree(new GrepOptions(), text).ConfigureAwait(false);

                this.fixture.Create<Mock<ISyntaxNodePresenter>>()
                    .Verify(x => x.Flush(), Times.Once());
            }

            private void WithParsed(string textToParseAsSyntaxTree)
            {
                this.fixture.Create<Mock<ICSharpSyntaxTreeStatic>>()
                    .Setup(x => x.ParseText(It.IsAny<string>()))
                    .Returns(CSharpSyntaxTree.ParseText(textToParseAsSyntaxTree));
            }

            private void VerifyResultContains(Func<SyntaxNode, bool> predicate, int times = 1)
            {
                this.fixture.Create<Mock<ISyntaxNodePresenter>>()
                    .Verify(
                        x => x.ProduceOutput(
                            It.Is<IEnumerable<SyntaxNode>>(
                                it => it.Where(predicate).Count() == times),
                            It.IsAny<GrepOptions>()), Times.Once());
            }
        }
    }
}
