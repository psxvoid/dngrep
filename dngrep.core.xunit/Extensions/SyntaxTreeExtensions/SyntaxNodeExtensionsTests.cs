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
        public class TryGetBody
        {
            [Fact]
            public void TryGetBody_MethodDeclarationBody_BlockSyntax()
            {
                const string target = "class C { int GetAge() { return 5; } }";

                AssertHasBody<MethodDeclarationSyntax, BlockSyntax>(target);
            }

            [Fact]
            public void TryGetBody_MethodDeclarationExpression_ExpressionSyntax()
            {
                const string target = "class C { int GetAge() => 5; }";

                AssertHasBody<MethodDeclarationSyntax, ArrowExpressionClauseSyntax>(target);
            }

            [Fact]
            public void TryGetBody_ConstructorDeclarationBody_BlockSyntax()
            {
                const string target = "using System; class C { C() { Console.WriteLine(5); } }";

                AssertHasBody<ConstructorDeclarationSyntax, BlockSyntax>(target);
            }

            [Fact]
            public void TryGetBody_ConstructorDeclarationExpression_ExpressionSyntax()
            {
                const string target = "using System; class C { C() => Console.WriteLine(5); }";

                AssertHasBody<ConstructorDeclarationSyntax, ArrowExpressionClauseSyntax>(target);
            }

            [Fact]
            public void TryGetBody_AnonimouseFunctionBody_BlockSyntax()
            {
                const string target = "using System; delegate(string s) { Console.WriteLine(s); };";

                AssertHasBody<AnonymousFunctionExpressionSyntax, BlockSyntax>(target);
            }

            [Fact]
            public void TryGetBody_AnonimouseFunctionExpression_ExpressionSyntax()
            {
                const string target = "using System; (string s) => Console.WriteLine(s);";

                AssertHasBody<AnonymousFunctionExpressionSyntax, InvocationExpressionSyntax>(target);
            }

            [Fact]
            public void TryGetBody_GetterBody_BlockSyntax()
            {
                const string target = "class C { int X { get { return 0; } } }";

                AssertHasBody<AccessorDeclarationSyntax, BlockSyntax>(target);
            }

            [Fact]
            public void TryGetBody_GetterExpression_ExpressionSyntax()
            {
                const string target = "class C { int X { get => 0; }";

                AssertHasBody<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax>(target);
            }

            [Fact]
            public void TryGetBody_SetterBody_BlockSyntax()
            {
                const string target = "class C { int x; int X { set { x = value; } } }";

                AssertHasBody<AccessorDeclarationSyntax, BlockSyntax>(target);
            }

            [Fact]
            public void TryGetBody_SetterExpression_ExpressionSyntax()
            {
                const string target = "class C { int x; int X { set => x = value; } }";

                SyntaxNode node =
                    AssertHasBody<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax>(target);

                // those ones are just to demonstrate that
                // when PropertyDeclarationSyntax's has getter or setter
                // then ExpressionBody property will be always set to null
                Assert.IsType<AccessorListSyntax>(node.Parent);
                Assert.IsType<PropertyDeclarationSyntax>(node?.Parent?.Parent);
                Assert.Null(((PropertyDeclarationSyntax?)(node?.Parent?.Parent))?.ExpressionBody);
            }

            [Fact]
            public void TryGetBody_ReadOnlyPropertyExpression_ExpressionSyntax()
            {
                const string target = "class C { int X => 0; }";

                SyntaxNode node =
                    AssertHasBody<PropertyDeclarationSyntax, ArrowExpressionClauseSyntax>(target);

                // those ones are just to demonstrate that
                // when PropertyDeclarationSyntax's has no getter and setter (read-only)
                // then AccessorList property will be always set to null
                Assert.IsType<PropertyDeclarationSyntax>(node);
                Assert.Null(((PropertyDeclarationSyntax?)node)?.AccessorList);
            }

            private static SyntaxNode AssertHasBody<TNode, TResult>(string targetCode)
                where TNode : SyntaxNode
                where TResult : SyntaxNode
            {
                SyntaxTree? tree = CSharpSyntaxTree.ParseText(targetCode);

                TNode nodeWithBody = tree.GetRoot().ChildNodes()
                    .GetNodesOfTypeRecursively<TNode>()
                    .First();

                Assert.IsType<TResult>(nodeWithBody.TryGetBody());

                return nodeWithBody;
            }
        }

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
                LocalDeclarationStatementSyntax node =
                    this.GetNode<LocalDeclarationStatementSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth.Spin.action", result);
            }

            [Fact]
            public void GetFullName_Method_MethodFullName()
            {
                MethodDeclarationSyntax? node = this.GetNode<MethodDeclarationSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth.Spin", result);
            }

            [Fact]
            public void GetFullName_Class_ClassFullName()
            {
                ClassDeclarationSyntax? node = this.GetNode<ClassDeclarationSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth", result);
            }

            [Fact]
            public void GetFullName_Namespace_NamespaceName()
            {
                NamespaceDeclarationSyntax node = this.GetNode<NamespaceDeclarationSyntax>();

                string result = node.GetFullName();

                Assert.Equal("SolarSystem", result);
            }

            [Fact]
            public void GetFullName_SyntaxBlock_ShouldThrow()
            {
                BlockSyntax node = this.GetNode<BlockSyntax>();

                Assert.Throws<InvalidOperationException>(() => node.GetFullName());
            }

            [Fact]
            public void TryGetFullName_Variable_VariableFullName()
            {
                LocalDeclarationStatementSyntax node =
                    this.GetNode<LocalDeclarationStatementSyntax>();

                string? result = node.GetFullName();

                Assert.Equal("SolarSystem.Earth.Spin.action", result);
            }

            [Fact]
            public void TryGetFullName_Method_MethodFullName()
            {
                MethodDeclarationSyntax node = this.GetNode<MethodDeclarationSyntax>();

                string? result = node.TryGetFullName();

                Assert.Equal("SolarSystem.Earth.Spin", result);
            }

            [Fact]
            public void TryGetFullName_Class_ClassFullName()
            {
                ClassDeclarationSyntax node = this.GetNode<ClassDeclarationSyntax>();

                string? result = node.TryGetFullName();

                Assert.Equal("SolarSystem.Earth", result);
            }

            [Fact]
            public void TryGetFullName_Namespace_NamespaceName()
            {
                NamespaceDeclarationSyntax node = this.GetNode<NamespaceDeclarationSyntax>();

                string? result = node.TryGetFullName();

                Assert.Equal("SolarSystem", result);
            }

            [Fact]
            public void TryGetFullName_SyntaxBlock_ShouldReturnNull()
            {
                BlockSyntax node = this.GetNode<BlockSyntax>();

                Assert.Null(node.TryGetFullName());
            }

            [Fact]
            public void TryGetFilePath_AnyBlock_ShouldReturnFilePath()
            {
                ClassDeclarationSyntax node = this.GetNode<ClassDeclarationSyntax>();

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
                ParameterSyntax node = this.GetNode<ParameterSyntax>();
                (int lineStart, int lineEnd, int charStart, int charEnd) =
                    node.GetSourceTextBounds();

                Assert.Equal(5, lineStart);
                Assert.Equal(5, lineEnd);
                Assert.Equal(43, charStart);
                Assert.Equal(58, charEnd);
            }

            [Fact]
            public void GetFirstParentOfType_ParentNotFound_Null()
            {
                ParameterSyntax node = this.GetNode<ParameterSyntax>();

                FieldDeclarationSyntax? actual =
                    node.GetFirstParentOfType<FieldDeclarationSyntax>();

                Assert.Null(actual);
            }

            [Fact]
            public void GetFirstParentOfType_ParentFound_Parent()
            {
                ParameterSyntax node = this.GetNode<ParameterSyntax>();
                ParameterListSyntax expected = this.GetNode<ParameterListSyntax>();

                ParameterListSyntax? actual = node.GetFirstParentOfType<ParameterListSyntax>();

                Assert.Equal(expected, actual);
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
