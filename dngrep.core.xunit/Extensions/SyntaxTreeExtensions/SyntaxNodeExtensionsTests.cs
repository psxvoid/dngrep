using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Xunit;
using System.Linq;
using System;

namespace dngrep.core.xunit.Extensions.SyntaxTreeExtensions
{
    public static class SyntaxNodeExtensionsTests
    {
        public class NamespaceClassMethodVariable
        {
            private const string SourceCode = @"
                namespace SolarSystem
                {
                    public class Earth
                    {
                        public string Spin()
                        {
                            string action = ""Spinning!"";
                            return string;
                        }
                    }
                }
            ";

            private readonly SyntaxTree syntaxTree;

            public NamespaceClassMethodVariable()
            {
                this.syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
            }

            [Fact]
            public void GetFullName_Variable_VariableFullName()
            {
                var node = this.GetNode<LocalDeclarationStatementSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth.Spin.action", result);
            }

            [Fact]
            public void GetFullName_Method_MethodFullName()
            {
                var node = this.GetNode<MethodDeclarationSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth.Spin", result);
            }

            [Fact]
            public void GetFullName_Class_ClassFullName()
            {
                var node = this.GetNode<ClassDeclarationSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth", result);
            }

            [Fact]
            public void GetFullName_Namespace_NamespaceName()
            {
                var node = this.GetNode<NamespaceDeclarationSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem", result);
            }

            [Fact]
            public void GetFullName_SyntaxBlock_ShouldThrow()
            {
                var node = this.GetNode<BlockSyntax>();

                Assert.Throws<InvalidOperationException>(() => node.GetFullName());
            }

            [Fact]
            public void TryGetFullName_Variable_VariableFullName()
            {
                var node = this.GetNode<LocalDeclarationStatementSyntax>();

                string? result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth.Spin.action", result);
            }

            [Fact]
            public void TryGetFullName_Method_MethodFullName()
            {
                var node = this.GetNode<MethodDeclarationSyntax>();

                string? result = node.TryGetFullName();

                Assert.Equal("SolarSystem.Earth.Spin", result);
            }

            [Fact]
            public void TryGetFullName_Class_ClassFullName()
            {
                var node = this.GetNode<ClassDeclarationSyntax>();

                string? result = node.TryGetFullName();

                Assert.Equal("SolarSystem.Earth", result);
            }

            [Fact]
            public void TryGetFullName_Namespace_NamespaceName()
            {
                var node = this.GetNode<NamespaceDeclarationSyntax>();

                string? result = node.TryGetFullName();

                Assert.Equal("SolarSystem", result);
            }

            [Fact]
            public void TryGetFullName_SyntaxBlock_ShouldReturnNull()
            {
                var node = this.GetNode<BlockSyntax>();

                Assert.Null(node.TryGetFullName());
            }

            private T GetNode<T>() where T : SyntaxNode
            {
                return this.syntaxTree
                    .GetRoot()
                    .ChildNodes()
                    .GetNodesOfTypeRecursively<T>()
                    .First();
            }
        }
    }
}
