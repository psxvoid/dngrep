using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Console;
using dngrep.tool.xunit.TestHelpers;
using Moq;
using Xunit;

namespace dngrep.tool.xunit.Console
{
    public class StandardInputReaderTests
    {
        private readonly IFixture fixture;

        private readonly StandardInputReader sut;

        public StandardInputReaderTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.fixture.Freeze<Mock<IStandardInputConsole>>();
            this.sut = this.fixture.Create<StandardInputReader>();
        }

        [Fact]
        public void IsInputRedirected_ConsoleInputRedirected_True()
        {
            this.fixture.Create<Mock<IStandardInputConsole>>()
                .SetupGet(x => x.IsInputRedirected).Returns(true);

            Assert.True(this.sut.IsInputRedirected());
        }

        [Fact]
        public void IsInputRedirected_ConsoleInputNotRedirected_False()
        {
            this.fixture.Create<Mock<IStandardInputConsole>>()
                .SetupGet(x => x.IsInputRedirected).Returns(false);

            Assert.False(this.sut.IsInputRedirected());
        }

        [Fact]
        public async Task ReadAsStringAsync_ConsoleInputRedirected_ShouldReturnResult()
        {
            const string result = "test";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(result));
            Mock<IStandardInputConsole> consoleMock =
                this.fixture.Create<Mock<IStandardInputConsole>>();
            consoleMock.SetupGet(x => x.IsInputRedirected).Returns(true);
            consoleMock.Setup(x => x.OpenStandardInput()).Returns(stream);

            Assert.Equal(result, await this.sut.ReadAsStringAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ReadAsStringAsync_ConsoleInputNotRedirected_ShouldThrow()
        {
            this.fixture.Create<Mock<IStandardInputConsole>>()
                .SetupGet(x => x.IsInputRedirected).Returns(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.sut.ReadAsStringAsync())
                .ConfigureAwait(false);
        }
    }
}
