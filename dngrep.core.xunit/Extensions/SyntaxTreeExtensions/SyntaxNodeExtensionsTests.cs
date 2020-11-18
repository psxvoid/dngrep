using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

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
                        public string Spin(double velocity, int multiplier)
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
                this.syntaxTree = CSharpSyntaxTree.ParseText(SourceCode, path: "x:/test.cs");
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

            [Fact]
            public void TryGetFilePath_AnyBlock_ShouldReturnFilePath()
            {
                var node = this.GetNode<ClassDeclarationSyntax>();

                Assert.Equal("x:/test.cs", node.TryGetFilePath());
            }

            [Fact]
            public void GetSourceTextBounds_NullNode_ShouldThrow()
            {
                Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    () => ((SyntaxNode)null).GetSourceTextBounds());
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            [Fact]
            public void GetSourceTextBounds_ShouldGetCorrectBounds()
            {
                var node = this.GetNode<ParameterSyntax>();
                var (lineStart, lineEnd, charStart, charEnd) = node.GetSourceTextBounds();

                Assert.Equal(5, lineStart);
                Assert.Equal(5, lineEnd);
                Assert.Equal(43, charStart);
                Assert.Equal(58, charEnd);
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
