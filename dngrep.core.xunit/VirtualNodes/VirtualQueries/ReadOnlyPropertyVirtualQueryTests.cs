using System;
using AutoFixture;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.VirtualNodes.VirtualQueries
{
    public class ReadOnlyPropertyVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly ReadOnlyPropertyVirtualQuery sut;

        public ReadOnlyPropertyVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<ReadOnlyPropertyVirtualQuery>();
        }

        [Fact]
        public void HasOverride_True()
        {
            Assert.True(this.sut.HasOverride);
        }

        [Fact]
        public void CanQuery_ReadOnlyProperty_True()
        {
            Assert.True(this.sut.CanQuery(CreateReadOnlyProperty()));
        }

        [Fact]
        public void CanQuery_GetSetProperty_False()
        {
            Assert.False(this.sut.CanQuery(CreateGetSetProperty()));
        }

        [Fact]
        public void CanQuery_AutoPropertyWithoutInitValue_False()
        {
            Assert.False(this.sut.CanQuery(CreateAutoPropertyWithoutInitValue()));
        }

        [Fact]
        public void CanQuery_AutoPropertyWithInitValue_False()
        {
            Assert.False(this.sut.CanQuery(CreateAutoPropertyWithInitValue()));
        }

        [Fact]
        public void CanQuery_GetOnlyExpresssionProperty_False()
        {
            Assert.False(this.sut.CanQuery(CreateGetOnlyExpressionPropertyDeclaration()));
        }

        [Fact]
        public void CanQuery_SetOnlyExpresssionProperty_False()
        {
            Assert.False(this.sut.CanQuery(CreateSetOnlyExpressionPropertyDeclaration()));
        }

        [Fact]
        public void Query_ReadOnlyProperty_Throws()
        {
            Assert.IsType<ReadOnlyPropertyDeclarationSyntax>(
                this.sut.Query(CreateReadOnlyProperty()));
        }

        [Fact]
        public void Query_GetSetProperty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateGetSetProperty()));
        }

        [Fact]
        public void Query_AutoPropertyWithoutInitValue_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateAutoPropertyWithoutInitValue()));
        }

        [Fact]
        public void Query_AutoPropertyWithInitValue_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateAutoPropertyWithInitValue()));
        }

        [Fact]
        public void Query_GetOnlyExpresssionProperty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateGetOnlyExpressionPropertyDeclaration()));
        }

        [Fact]
        public void Query_SetOnlyExpresssionProperty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateSetOnlyExpressionPropertyDeclaration()));
        }

        [Fact]
        public void Query_NotProperty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(SyntaxFactory.ClassDeclaration("any")));
        }

        [Fact]
        public void Query_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => this.sut.Query(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        private static SyntaxNode CreateGetSetProperty()
        {
            const string sourceText =
                "public class C1 { int x; int X { get { return x; } set { x = value; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }

        private static SyntaxNode CreateAutoPropertyWithoutInitValue()
        {
            const string sourceText = "public class C1 { int X { get; set; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }

        private static SyntaxNode CreateAutoPropertyWithInitValue()
        {
            const string sourceText = "public class C2 { int X { get; set; } = 5; }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }

        private static SyntaxNode CreateReadOnlyProperty()
        {
            const string sourceText = "public class C2 { int X => 5; }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }

        private static SyntaxNode CreateGetOnlyExpressionPropertyDeclaration()
        {
            const string sourceText = "public class C4 { int X { get => 5; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }

        private static SyntaxNode CreateSetOnlyExpressionPropertyDeclaration()
        {
            const string sourceText = "public class C4 { int x; int X { set => x = value; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }
    }
}
