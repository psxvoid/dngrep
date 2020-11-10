using AutoFixture;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Core.Options;
using dngrep.tool.Core.Output.Presenters;
using dngrep.tool.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Xunit;

namespace dngrep.tool.xunit.Core.Output.Presenters
{
    public class SyntaxNodeStatisticsPresenterTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IConsole> consoleMock;
        private readonly SyntaxNodeStatisticsConsolePresenter sut;

        public SyntaxNodeStatisticsPresenterTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.consoleMock = this.fixture.Freeze<Mock<IConsole>>();

            this.sut = this.fixture.Create<SyntaxNodeStatisticsConsolePresenter>();
        }

        [Fact]
        public void ProduceOutput_SingleClass_ShouldPrintClassCount()
        {
            var nodes = new[] { SyntaxFactory.ClassDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Classes:\t1")), Times.Once());
        }


        [Fact]
        public void ProduceOutput_SingleClass_ShouldPrintOnlyClassCount()
        {
            var nodes = new[] { SyntaxFactory.ClassDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleNamespace_ShouldPrintNamespaceCount()
        {
            var nodes = new[] {
                SyntaxFactory.NamespaceDeclaration(
                    SyntaxFactory.ParseName("ns"))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Namespaces:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleNamespace_ShouldPrintOnlyNamespaceCount()
        {
            var nodes = new[] {
                SyntaxFactory.NamespaceDeclaration(
                    SyntaxFactory.ParseName("ns"))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleMethod_ShouldPrintMethodCount()
        {
            var nodes = new[] {
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Methods:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleMethod_ShouldPrintOnlyMethodCount()
        {
            var nodes = new[] {
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleLocalVariable_ShouldPrintLocalVariableCount()
        {
            var nodes = new[] {
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Local Variables:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleLocalVariable_ShouldPrintOnlyLocalVariableCount()
        {
            var nodes = new[] {
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleArgument_ShouldPrintArgumentCount()
        {
            var nodes = new[] {
                SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("any"),
                        SyntaxFactory.IdentifierName("any")))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Arguments:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleArgument_ShouldPrintOnlyArgumentCount()
        {
            var nodes = new[] {
                SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("any"),
                        SyntaxFactory.IdentifierName("any")))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleEnum_ShouldPrintEnumCount()
        {
            var nodes = new[] { SyntaxFactory.EnumDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Enums:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleEnum_ShouldPrintOnlyEnumCount()
        {
            var nodes = new[] { SyntaxFactory.EnumDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleInterface_ShouldPrintInterfaceCount()
        {
            var nodes = new[] { SyntaxFactory.InterfaceDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Interfaces:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleInterface_ShouldPrintOnlyInterfaceCount()
        {
            var nodes = new[] { SyntaxFactory.InterfaceDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleStruct_ShouldPrintStructCount()
        {
            var nodes = new[] { SyntaxFactory.StructDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Structs:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleStruct_ShouldPrintOnlyStructCount()
        {
            var nodes = new[] { SyntaxFactory.StructDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleField_ShouldPrintFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Fields:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleField_ShouldPrintOnlyFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleProperty_ShouldPrintFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Properties:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutput_SingleProperty_ShouldPrintOnlyFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutput_EveryType_ShouldPrintCountsForAllTypes()
        {
            var nodes = new SyntaxNode[] {
                SyntaxFactory.ClassDeclaration("any"),
                SyntaxFactory.NamespaceDeclaration(
                    SyntaxFactory.ParseName("ns")) ,
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any"),
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") }))),
                SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("any"),
                        SyntaxFactory.IdentifierName("any"))),
                SyntaxFactory.EnumDeclaration("any"),
                SyntaxFactory.InterfaceDeclaration("any")
            };


            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Classes:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Namespaces:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Methods:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Local Variables:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Arguments:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Enums:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Interfaces:\t1")), Times.Once());
        }
    }
}
