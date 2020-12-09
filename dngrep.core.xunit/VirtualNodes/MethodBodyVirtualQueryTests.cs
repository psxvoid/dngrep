using System;
using System.Linq;
using AutoFixture;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
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
        public void HasOverride_False()
        {
            Assert.False(this.sut.HasOverride);
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
            Assert.True(this.sut.CanQuery(CreateMethodWithBody()));
        }

        [Fact]
        public void CanQuery_SupportedTypeAndNonEmptyExpressionBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateMethodWithExpressionBody()));
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
            IVirtualSyntaxNode result = this.sut.Query(CreateMethodWithBody());

            Assert.IsType<MethodBodyDeclarationSyntax>(result);
            Assert.IsType<BlockSyntax>(result.BaseNode);
            Assert.Equal(VirtualSyntaxNodeKind.MethodBody, result.Kind);
        }

        [Fact]
        public void Query_SupportedTypeAndHasExpressionBody_MethodBodyDeclarationSyntax()
        {
            IVirtualSyntaxNode result = this.sut.Query(CreateMethodWithExpressionBody());

            Assert.IsType<MethodBodyDeclarationSyntax>(result);
            Assert.IsType<ArrowExpressionClauseSyntax>(result.BaseNode);
            Assert.Equal(VirtualSyntaxNodeKind.MethodBody, result.Kind);
        }

        private static MethodDeclarationSyntax CreateMethodWithoutBody()
        {
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("bool"), "any");
        }

        private static MethodDeclarationSyntax CreateMethodWithBody()
        {
            return SyntaxFactory.MethodDeclaration(
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
        }

        private static MethodDeclarationSyntax CreateMethodWithExpressionBody()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText("class C { int GetNum() => 5; }");
            return tree.GetRoot()
                .ChildNodes()
                .GetNodesOfTypeRecursively<MethodDeclarationSyntax>()
                .First();
        }
    }
}
