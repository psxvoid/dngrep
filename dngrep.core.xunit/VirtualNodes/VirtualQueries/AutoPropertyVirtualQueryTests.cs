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
    public class AutoPropertyVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly AutoPropertyVirtualQuery sut;

        public AutoPropertyVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<AutoPropertyVirtualQuery>();
        }

        [Fact]
        public void HasOverride_True()
        {
            Assert.True(this.sut.HasOverride);
        }

        [Fact]
        public void CanQuery_AutoPropertyWithoutInitValue_True()
        {
            Assert.True(this.sut.CanQuery(CreateAutoPropertyWithoutInitValue()));
        }

        [Fact]
        public void CanQuery_AutoPropertyWithInitValue_True()
        {
            Assert.True(this.sut.CanQuery(CreateAutoPropertyWithInitValue()));
        }

        [Fact]
        public void CanQuery_NotAutoProperty_False()
        {
            Assert.False(this.sut.CanQuery(CreateNotAutoPropertyDeclaration()));
        }

        [Fact]
        public void Query_AutoPropertyWithoutInitValue_VirtualNode()
        {
            Assert.IsType<AutoPropertyDeclarationSyntax>(
                this.sut.Query(CreateAutoPropertyWithoutInitValue()));
        }

        [Fact]
        public void Query_AutoPropertyWithInitValue_VirtualNode()
        {
            Assert.IsType<AutoPropertyDeclarationSyntax>(
                this.sut.Query(CreateAutoPropertyWithInitValue()));
        }

        [Fact]
        public void Query_NotAutoProperty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateNotAutoPropertyDeclaration()));
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

        private static SyntaxNode CreateNotAutoPropertyDeclaration()
        {
            const string sourceText = "public class C4 { int X { get => 5; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<PropertyDeclarationSyntax>();
        }
    }
}
