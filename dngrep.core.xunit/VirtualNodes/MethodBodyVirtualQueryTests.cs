using System;
using System.Linq;
using AutoFixture;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.VirtualNodes
{
    public class MethodBodyVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly MethodBodyVirtualQuery sut;

        public MethodBodyVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<MethodBodyVirtualQuery>();
        }

        [Fact]
        public void HasOverride_True()
        {
            Assert.True(this.sut.HasOverride);
        }

        [Fact]
        public void CanQuery_Null_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => this.sut.CanQuery(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void CanQuery_NotSupportedType_False()
        {
            Assert.False(this.sut.CanQuery(SyntaxFactory.ClassDeclaration("any")));
        }

        [Fact]
        public void CanQuery_SupportedTypeAndEmptyBody_False()
        {
            Assert.False(this.sut.CanQuery(CreateMethodWithoutBody()));
        }

        [Fact]
        public void CanQuery_SupportedTypeAndNonEmptyBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateMethodBody()));
        }

        [Fact]
        public void CanQuery_SupportedTypeAndNonEmptyExpressionBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateMethodArrowExpressionBody()));
        }

        [Fact]
        public void CanQuery_SupportedTypeAndNonEmptyAnonymousBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateAnonymousMethodBody()));
        }

        [Fact]
        public void CanQuery_SupportedTypeAndNonEmptyAnonymousExpressionBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateAnonymousMethodExpressionBody()));
        }

        [Fact]
        public void CanQuery_IfStatementSyntaxCondition_False()
        {
            Assert.False(this.sut.CanQuery(CreateIfCondition()));
        }

        [Fact]
        public void CanQuery_IfStatementSyntaxStatement_False()
        {
            Assert.False(this.sut.CanQuery(CreateIfStatementSyntaxStatement()));
        }

        [Fact]
        public void Query_Null_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => this.sut.Query(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Query_NotSupportedType_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(SyntaxFactory.ClassDeclaration("any")));
        }

        [Fact]
        public void Query_SupportedTypeAndNoBody_Throws()
        {
            Assert.Throws<InvalidOperationException>(
                () => this.sut.Query(CreateMethodWithoutBody()));
        }

        [Fact]
        public void Query_SupportedTypeAndHasBody_MethodBodyDeclarationSyntax()
        {
            IVirtualSyntaxNode result = this.sut.Query(CreateMethodBody());

            Assert.IsType<MethodBodyDeclarationSyntax>(result);
            Assert.IsType<BlockSyntax>(result.BaseNode);
            Assert.Equal(VirtualSyntaxNodeKind.MethodBody, result.Kind);
        }

        [Fact]
        public void Query_SupportedTypeAndHasExpressionBody_MethodBodyDeclarationSyntax()
        {
            IVirtualSyntaxNode result = this.sut.Query(CreateMethodArrowExpressionBody());

            Assert.IsType<MethodBodyDeclarationSyntax>(result);
            Assert.IsType<ArrowExpressionClauseSyntax>(result.BaseNode);
            Assert.Equal(VirtualSyntaxNodeKind.MethodBody, result.Kind);
        }

        [Fact]
        public void Query_SupportedTypeAndHasAnonymousBody_MethodBodyDeclarationSyntax()
        {
            IVirtualSyntaxNode result = this.sut.Query(CreateMethodBody());

            Assert.IsType<MethodBodyDeclarationSyntax>(result);
            Assert.IsType<BlockSyntax>(result.BaseNode);
            Assert.Equal(VirtualSyntaxNodeKind.MethodBody, result.Kind);
        }

        [Fact]
        public void Query_SupportedTypeAndHasAnonymousExpressionBody_MethodBodyDeclarationSyntax()
        {
            IVirtualSyntaxNode result = this.sut.Query(CreateAnonymousMethodExpressionBody());

            Assert.IsType<MethodBodyDeclarationSyntax>(result);
            Assert.IsType<InvocationExpressionSyntax>(result.BaseNode);
            Assert.Equal(VirtualSyntaxNodeKind.MethodBody, result.Kind);
        }

        private static BlockSyntax CreateMethodWithoutBody()
        {
            BlockSyntax body = SyntaxFactory.Block();
            BlockSyntax blockParent = SyntaxFactory.Block(body);

#pragma warning disable CS8603 // Possible null reference return.
            return blockParent.ChildNodes().First() as BlockSyntax;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private static ExpressionSyntax CreateIfCondition()
        {
            const string sourceText = "class C { void M() { if (true) {} }}";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<IfStatementSyntax>().Condition;
        }

        private static StatementSyntax CreateIfStatementSyntaxStatement()
        {
            const string sourceText = "class C { void M() { if (true) {} }}";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<IfStatementSyntax>().Statement;
        }

        private static BlockSyntax CreateMethodBody()
        {
            MethodDeclarationSyntax parent = SyntaxFactory.MethodDeclaration(
                attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                modifiers: SyntaxFactory.TokenList(),
                returnType: SyntaxFactory.ParseTypeName("bool"),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                explicitInterfaceSpecifier: null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                identifier: SyntaxFactory.Identifier("MyMethod"),
                typeParameterList: SyntaxFactory.TypeParameterList(),
                parameterList: SyntaxFactory.ParameterList(),
                constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                body: SyntaxFactory.Block(),
                semicolonToken: new SyntaxToken());

#pragma warning disable CS8603 // Possible null reference return.
            return parent.Body;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private static ArrowExpressionClauseSyntax CreateMethodArrowExpressionBody()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText("class C { int Get(int x) => 5 + x; }");
            MethodDeclarationSyntax parent = tree.GetRoot()
                .ChildNodes()
                .GetNodesOfTypeRecursively<MethodDeclarationSyntax>()
                .First();

#pragma warning disable CS8603 // Possible null reference return.
            return parent.ExpressionBody;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private static CSharpSyntaxNode CreateAnonymousMethodBody()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
            class C {
                void Method()
                {
                    Action<int> f = (x) => { Console.WriteLine(x); };
                    f(5);
                }
            }");

            ParenthesizedLambdaExpressionSyntax? parent = tree.GetRoot()
                ?.ChildNodes()
                ?.GetNodesOfTypeRecursively<MethodDeclarationSyntax>()
                ?.First()
                ?.Body
                ?.ChildNodes()
                ?.GetNodesOfTypeRecursively<ParenthesizedLambdaExpressionSyntax>()
                ?.First();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return parent.Body;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private static ExpressionSyntax CreateAnonymousMethodExpressionBody()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
            class C {
                void Method()
                {
                    Action<int> f = (x) => Console.WriteLine(x);
                    f(5);
                }
            }");

            ParenthesizedLambdaExpressionSyntax? parent = tree.GetRoot()
                ?.ChildNodes()
                ?.GetNodesOfTypeRecursively<MethodDeclarationSyntax>()
                ?.First()
                ?.Body
                ?.ChildNodes()
                ?.GetNodesOfTypeRecursively<ParenthesizedLambdaExpressionSyntax>()
                ?.First();

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return parent.ExpressionBody;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
