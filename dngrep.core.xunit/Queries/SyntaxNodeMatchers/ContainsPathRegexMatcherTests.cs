using System;
using AutoFixture;
using dngrep.core.Queries.SyntaxNodeMatchers;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.Queries.SyntaxNodeMatchers
{
    public class ContainsPathRegexMatcherTests
    {
        private readonly IFixture fixture;

        public ContainsPathRegexMatcherTests()
        {
            this.fixture = AutoFixtureFactory.Default();
        }

        [Fact]
        public void Constructor_NullSubString_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new ContainsPathRegexMatcher(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Constructor_EmptySubString_Throws()
        {
            Assert.Throws<ArgumentException>(() => new ContainsPathRegexMatcher(""));
        }

        [Fact]
        public void Constructor_WhitespaceSubString_Throws()
        {
            Assert.Throws<ArgumentException>(() => new ContainsPathRegexMatcher("\t\n"));
        }

        [Fact]
        public void Match_FullMatch_True()
        {
            this.fixture.Inject("x:/test.cs");
            var sut = this.fixture.Create<ContainsPathRegexMatcher>();

            Assert.True(sut.Match(CreateNodeWithPath("x:/test.cs")));
        }

        [Fact]
        public void Match_PartialMatch_True()
        {
            this.fixture.Inject("test");
            var sut = this.fixture.Create<ContainsPathRegexMatcher>();

            Assert.True(sut.Match(CreateNodeWithPath("x:/test.cs")));
        }

        [Fact]
        public void Match_RegexMatch_True()
        {
            this.fixture.Inject(@".:/.{4,4}\.cs");
            var sut = this.fixture.Create<ContainsPathRegexMatcher>();

            Assert.True(sut.Match(CreateNodeWithPath("x:/test.cs")));
        }

        [Fact]
        public void Match_NoMatch_False()
        {
            this.fixture.Inject("test.vb");
            var sut = this.fixture.Create<ContainsPathRegexMatcher>();

            Assert.False(sut.Match(CreateNodeWithPath("x:/test.cs")));
        }

        private static SyntaxNode CreateNodeWithPath(string? path)
        {

            var syntaxTree = SyntaxFactory.SyntaxTree(
                SyntaxFactory.CompilationUnit(
                    new SyntaxList<ExternAliasDirectiveSyntax>(),
                    new SyntaxList<UsingDirectiveSyntax>(),
                    new SyntaxList<AttributeListSyntax>(),
                    new SyntaxList<MemberDeclarationSyntax>(
                        SyntaxFactory.ClassDeclaration("any"))),
                options: null,
#pragma warning disable CS8604 // Possible null reference argument.
                path: path);
#pragma warning restore CS8604 // Possible null reference argument.

            return syntaxTree.GetRoot();
        }
    }
}
