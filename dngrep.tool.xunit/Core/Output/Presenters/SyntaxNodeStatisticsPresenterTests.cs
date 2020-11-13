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
        private readonly Mock<IStringConsole> consoleMock;
        private readonly SyntaxNodeStatisticsConsolePresenter sut;

        public SyntaxNodeStatisticsPresenterTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.consoleMock = this.fixture.Freeze<Mock<IStringConsole>>();

            this.sut = this.fixture.Create<SyntaxNodeStatisticsConsolePresenter>();
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleClass_ShouldPrintClassCount()
        {
            var nodes = new[] { SyntaxFactory.ClassDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Classes:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleClassTwice_ShouldPrintClassCount()
        {
            var nodes = new[] { SyntaxFactory.ClassDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Classes:\t2")), Times.Once());
        }


        [Fact]
        public void ProduceOutputAndFlash_SingleClass_ShouldPrintOnlyClassCount()
        {
            var nodes = new[] { SyntaxFactory.ClassDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleNamespace_ShouldPrintNamespaceCount()
        {
            var nodes = new[] {
                SyntaxFactory.NamespaceDeclaration(
                    SyntaxFactory.ParseName("ns"))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Namespaces:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleNamespace_ShouldPrintOnlyNamespaceCount()
        {
            var nodes = new[] {
                SyntaxFactory.NamespaceDeclaration(
                    SyntaxFactory.ParseName("ns"))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleMethod_ShouldPrintMethodCount()
        {
            var nodes = new[] {
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Methods:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleMethod_ShouldPrintOnlyMethodCount()
        {
            var nodes = new[] {
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleLocalVariable_ShouldPrintLocalVariableCount()
        {
            var nodes = new[] {
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Local Vars:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleLocalVariable_ShouldPrintOnlyLocalVariableCount()
        {
            var nodes = new[] {
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleArgument_ShouldPrintArgumentCount()
        {
            var nodes = new[] {
                SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("any"),
                        SyntaxFactory.IdentifierName("any")))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Arguments:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleArgument_ShouldPrintOnlyArgumentCount()
        {
            var nodes = new[] {
                SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("any"),
                        SyntaxFactory.IdentifierName("any")))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleEnum_ShouldPrintEnumCount()
        {
            var nodes = new[] { SyntaxFactory.EnumDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Enums:\t\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleEnum_ShouldPrintOnlyEnumCount()
        {
            var nodes = new[] { SyntaxFactory.EnumDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleInterface_ShouldPrintInterfaceCount()
        {
            var nodes = new[] { SyntaxFactory.InterfaceDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Interfaces:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleInterface_ShouldPrintOnlyInterfaceCount()
        {
            var nodes = new[] { SyntaxFactory.InterfaceDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleStruct_ShouldPrintStructCount()
        {
            var nodes = new[] { SyntaxFactory.StructDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Structs:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleStruct_ShouldPrintOnlyStructCount()
        {
            var nodes = new[] { SyntaxFactory.StructDeclaration("any") };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleField_ShouldPrintFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Fields:\t\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleField_ShouldPrintOnlyFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleProperty_ShouldPrintFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Properties:\t1")), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_SingleProperty_ShouldPrintOnlyFieldCount()
        {
            var nodes = new[] {
                SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        "any")
            };

            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

            this.consoleMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void ProduceOutputAndFlash_EveryType_ShouldPrintCountsForAllTypes()
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
                SyntaxFactory.InterfaceDeclaration("any"),
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))
            };


            this.sut.ProduceOutput(nodes, this.fixture.Create<GrepOptions>());
            this.sut.Flush();

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
                    It.Is<string>(it => it == "Local Vars:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Arguments:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Enums:\t\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Interfaces:\t1")), Times.Once());
            this.consoleMock.Verify(
                x => x.WriteLine(
                    It.Is<string>(it => it == "Fields:\t\t1")), Times.Once());
        }
    }
}
