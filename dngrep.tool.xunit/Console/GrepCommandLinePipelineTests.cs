using System.Threading.Tasks;
using AutoFixture;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Console;
using dngrep.tool.Core;
using dngrep.tool.Core.Options;
using dngrep.tool.xunit.TestHelpers;
using Moq;
using Xunit;

namespace dngrep.tool.xunit.Console
{
    public class GrepCommandLinePipelineTests
    {
        private readonly IFixture fixture;
        private readonly GrepCommandLinePipeline sut;

        public GrepCommandLinePipelineTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.fixture.Freeze<Mock<IProjectGrep>>();
            var parser = this.fixture.Freeze<Mock<IParser>>();

            var options = this.fixture.Freeze<Mock<IParserResult<GrepOptions>>>();
            parser.Setup(x => x.ParseArguments<GrepOptions>(It.IsAny<string[]>()))
                .Returns(options.Object);

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
    }
}
