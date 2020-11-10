using System;
using AutoFixture;
using dngrep.tool.Core.Output.Presenters;
using dngrep.tool.xunit.TestHelpers;
using Lamar;
using Moq;
using Xunit;

namespace dngrep.tool.xunit.Core.Output.Presenters
{
    public class PresenterFactoryTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IContainer> containerMock;
        private readonly PresenterFactory sut;

        public PresenterFactoryTests()
        {
            this.fixture = AutoFixtureFactory.Default();

            this.containerMock = this.fixture.Freeze<Mock<IContainer>>();
            this.containerMock.Setup(x => x.GetInstance(It.IsAny<Type>()))
                .Returns(new Mock<ISyntaxNodePresenter>().Object);

            this.sut = this.fixture.Create<PresenterFactory>();
        }

        [Fact]
        public void GetPresenter_Search_ConsoleSyntaxNodePresenter()
        {
            this.sut.GetPresenter(PresenterKind.Search);

            this.containerMock.Verify(
                x => x.GetInstance(
                    It.Is<Type>(
                        it => it == typeof(ConsoleSyntaxNodePresenter))),
                Times.Once());
            this.containerMock.Verify(x => x.GetInstance(It.IsAny<Type>()), Times.Once());
        }

        [Fact]
        public void GetPresenter_Statistics_ConsoleSyntaxNodePresenter()
        {
            this.sut.GetPresenter(PresenterKind.Statistics);

            this.containerMock.Verify(
                x => x.GetInstance(
                    It.Is<Type>(
                        it => it == typeof(SyntaxNodeStatisticsConsolePresenter))),
                Times.Once());
            this.containerMock.Verify(x => x.GetInstance(It.IsAny<Type>()), Times.Once());
        }

        [Fact]
        public void GetPresenter_InstanceIsPresenter_ShouldReturnInstanceFromContainer()
        {
            object instance = new Mock<ISyntaxNodePresenter>().Object;
            this.containerMock.Setup(x => x.GetInstance(It.IsAny<Type>())).Returns(instance);

            ISyntaxNodePresenter? result = this.sut.GetPresenter(PresenterKind.Statistics);

            Assert.Equal(instance, result);
        }

        [Fact]
        public void GetPresenter_InstanceIsNotPresenter_ShouldThrow()
        {
            object instance = new object();
            this.containerMock.Setup(x => x.GetInstance(It.IsAny<Type>())).Returns(instance);

            Assert.Throws<InvalidOperationException>(
                () => this.sut.GetPresenter(PresenterKind.Statistics));
        }
    }
}
