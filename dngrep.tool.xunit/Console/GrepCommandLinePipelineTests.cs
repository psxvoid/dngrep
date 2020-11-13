using System;
using System.Threading.Tasks;
using AutoFixture;
using dngrep.tool.Abstractions.Build;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Console;
using dngrep.tool.Core;
using dngrep.tool.Core.Exceptions;
using dngrep.tool.Core.Options;
using dngrep.tool.xunit.TestHelpers;
using Moq;
using Xunit;

namespace dngrep.tool.xunit.Console
{
    public class GrepCommandLinePipelineTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IStandardInputReader> inputMock;
        private readonly GrepCommandLinePipeline sut;

        public GrepCommandLinePipelineTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.fixture.Freeze<Mock<IProjectGrep>>();
            var parser = this.fixture.Freeze<Mock<IParser>>();

            var options = this.fixture.Freeze<Mock<IParserResult<GrepOptions>>>();
            parser.Setup(x => x.ParseArguments<GrepOptions>(It.IsAny<string[]>()))
                .Returns(options.Object);
            options.Setup(x => x.WithParsedAsync(It.IsAny<Func<GrepOptions, Task>>()))
                .Callback<Func<GrepOptions, Task>>(
                    c => c.Invoke(this.fixture.Create<GrepOptions>()).Wait());
            this.inputMock = this.fixture.Freeze<Mock<IStandardInputReader>>();
            this.inputMock.Setup(x => x.IsInputRedirected()).Returns(false);

            this.fixture.Freeze<Mock<IMSBuildLocator>>();
            this.fixture.Freeze<Mock<IStringConsole>>();

            this.sut = this.fixture.Create<GrepCommandLinePipeline>();
        }

        [Fact]
        public async Task NonEmptyArguments_ShouldPassArgumentsToParser()
        {
            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.fixture.Create<Mock<IParser>>()
                .Verify(x => x.ParseArguments<GrepOptions>(
                    It.Is<string[]>(it => it[0] == "arg1" && it[1] == "arg2")), Times.Once());
        }

        [Fact]
        public async Task NonEmptyArguments_ShouldRunParsedResultWithProjectGrep()
        {
            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.fixture.Create<Mock<IParserResult<GrepOptions>>>()
                .Verify(x => x.WithParsedAsync(
                    this.fixture.Create<Mock<IProjectGrep>>().Object.FolderAsync
                ), Times.Once());
        }

        [Fact]
        public async Task NonEmptyArguments_ShouldRegisterBuildLocatorDefaults()
        {
            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.fixture.Create<Mock<IMSBuildLocator>>()
                .Verify(x => x.RegisterDefaults(), Times.Once());
        }

        [Fact]
        public async Task NonEmptyArgumentsAndGrepException_ShouldPrintGrepErrorMessage()
        {
            const string message = "Sorry, shutting down.";
            this.fixture.Create<Mock<IProjectGrep>>()
                .Setup(x => x.FolderAsync(It.IsAny<GrepOptions>()))
                .Throws(new GrepException(message));

            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.fixture.Create<Mock<IStringConsole>>()
                .Verify(x => x.WriteLine(
                    It.Is<string>(it => it == message)));
        }

        [Fact]
        public async Task InputHasData_ShouldReadInputAsString()
        {
            this.inputMock.Setup(x => x.IsInputRedirected()).Returns(true);

            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.inputMock.Verify(x => x.ReadAsStringAsync(), Times.Once());
        }

        [Fact]
        public async Task InputHasData_BuildLocatorShouldNotBeRegisterDefaults()
        {
            this.inputMock.Setup(x => x.IsInputRedirected()).Returns(true);

            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.fixture.Create<Mock<IMSBuildLocator>>()
                .Verify(x => x.RegisterDefaults(), Times.Never);
        }

        [Fact]
        public async Task InputHasDataAndResult_ShouldGrepText()
        {
            const string standardInputResult = "code";
            this.inputMock.Setup(x => x.IsInputRedirected()).Returns(true);
            this.inputMock.Setup(
                x => x.ReadAsStringAsync()).Returns(Task.FromResult(standardInputResult));

            await this.sut.ParseArgsAndRun(new[] { "arg1", "arg2" }).ConfigureAwait(false);

            this.fixture.Create<Mock<IProjectGrep>>()
                .Verify(
                    x => x.TextAsync(
                        It.IsAny<GrepOptions>(),
                        It.Is<string>(it => it == standardInputResult)),
                    Times.Once());
        }
    }
}
