using System.Linq;
using dngrep.core.Extensions.SourceTextExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries.SyntaxNodeMatchers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.Queries.SyntaxNodeMatchers
{
    public static class SourceTextPositionMatcherTests
    {
        public class SingleSourceLine
        {
            private const string SourceText = "class Person { int age; }";
            private readonly SyntaxTree tree;

            public SingleSourceLine()
            {
                this.tree = CSharpSyntaxTree.ParseText(SourceText);
            }

            [Fact]
            public void Match_ClassLocationAndClassNode_True()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(0, 0));

                Assert.True(matcher.Match(this.GetNode<ClassDeclarationSyntax>()));
            }

            [Fact]
            public void Match_ClassLocationAndFieldNode_False()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(0, 0));

                Assert.False(matcher.Match(this.GetNode<FieldDeclarationSyntax>()));
            }

            [Fact]
            public void Match_FieldLocationAndClassNode_True()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(0, 15));

                Assert.True(matcher.Match(this.GetNode<ClassDeclarationSyntax>()));
            }

            [Fact]
            public void Match_FieldLocationAndFieldNode_True()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(0, 15));

                Assert.True(matcher.Match(this.GetNode<FieldDeclarationSyntax>()));
            }

            private T GetNode<T>() where T : SyntaxNode
            {
                return this.tree
                    .GetRoot()
                    .ChildNodes()
                    .GetNodesOfTypeRecursively<T>()
                    .First();
            }
        }

        public class MultipleSourceLines
        {
            private const string SourceText = 
                "class Person\n"  +
                "{\n" + 
                 "int age;\n" +
                "}\n";

            private readonly SyntaxTree tree;

            public MultipleSourceLines()
            {
                this.tree = CSharpSyntaxTree.ParseText(SourceText);
            }

            [Fact]
            public void Match_ClassLocationAndClassNode_True()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(0, 0));

                Assert.True(matcher.Match(this.GetNode<ClassDeclarationSyntax>()));
            }

            [Fact]
            public void Match_ClassLocationAndFieldNode_False()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(0, 0));

                Assert.False(matcher.Match(this.GetNode<FieldDeclarationSyntax>()));
            }

            [Fact]
            public void Match_FieldLocationAndClassNode_True()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(2, 0));

                Assert.True(matcher.Match(this.GetNode<ClassDeclarationSyntax>()));
            }

            [Fact]
            public void Match_FieldLocationAndFieldNode_True()
            {
                var matcher = new SourceTextPositionMatcher(
                    this.tree.GetText().GetSingleCharSpan(2, 0));

                Assert.True(matcher.Match(this.GetNode<FieldDeclarationSyntax>()));
            }

            private T GetNode<T>() where T : SyntaxNode
            {
                return this.tree
                    .GetRoot()
                    .ChildNodes()
                    .GetNodesOfTypeRecursively<T>()
                    .First();
            }
        }
    }
}
