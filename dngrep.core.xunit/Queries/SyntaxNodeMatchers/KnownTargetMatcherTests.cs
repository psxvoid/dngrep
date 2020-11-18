using System;
using dngrep.core.Queries.SyntaxNodeMatchers;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace dngrep.core.xunit.Queries.SyntaxNodeMatchers
{
    public class KnownTargetMatcherTests
    {
        private readonly KnownTargetMatcher sut;

        public KnownTargetMatcherTests()
        {
            this.sut = KnownTargetMatcher.Instance;
        }
        
        [Fact]
        public void Match_Null_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => this.sut.Match(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Match_Destructor_False()
        {
            Assert.False(this.sut.Match(SyntaxFactory.DestructorDeclaration("any")));
        }

        [Fact]
        public void Match_Class_True()
        {
            Assert.True(this.sut.Match(SyntaxFactory.ClassDeclaration("any")));
        }

        [Fact]
        public void Match_Struct_True()
        {
            Assert.True(this.sut.Match(SyntaxFactory.StructDeclaration("any")));
        }

        [Fact]
        public void Match_Enum_True()
        {
            Assert.True(this.sut.Match(SyntaxFactory.EnumDeclaration("any")));
        }

        [Fact]
        public void Match_Field_True()
        {
            Assert.True(this.sut.Match(
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName("bool"),
                        SyntaxFactory.SeparatedList(
                            new[] { SyntaxFactory.VariableDeclarator("any") })))));
        }

        [Fact]
        public void Match_Interface_True()
        {
            Assert.True(this.sut.Match(SyntaxFactory.InterfaceDeclaration("any")));
        }

        [Fact]
        public void Match_Method_True()
        {
            Assert.True(this.sut.Match(
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any")));
        }

        [Fact]
        public void Match_MethodParameter_True()
        {
            Assert.True(this.sut.Match(
                SyntaxFactory.Parameter(
                   SyntaxFactory.Identifier("any"))
                .WithType(SyntaxFactory.ParseTypeName("bool"))));
        }


        [Fact]
        public void Match_InvocationArgument_True()
        {
            Assert.True(this.sut.Match(
                SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("any"),
                        SyntaxFactory.IdentifierName("any")))));
        }

        [Fact]
        public void Match_Namespace_True()
        {
            Assert.True(this.sut.Match(
                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("any"))));
        }

        [Fact]
        public void Match_Property_True()
        {
            Assert.True(this.sut.Match(
                SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.ParseTypeName("bool"),
                    "any")));
        }
    }
}
