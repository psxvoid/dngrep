using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace dngrep.core.xunit.Queries.Targets.BaseTests
{
    public abstract class SyntaxNodeMatcherTestBase
    {
        protected abstract ISyntaxNodeMatcher Sut { get; }

        protected void AssertMatch<TNode>(
            string targetCode,
            bool expected,
            Func<TNode, SyntaxNode>? selectNode = null)
            where TNode : SyntaxNode
        {
            SyntaxTree? tree = CSharpSyntaxTree.ParseText(targetCode);

            TNode root = tree.GetRoot().GetFirstChildOfTypeRecursively<TNode>();

            SyntaxNode target = selectNode != null
                ? selectNode(root)
                : root;

            Assert.Equal(expected, this.Sut.Match(target));
        }

        protected void AssertMatch<TNode>(
            string targetCode,
            bool expected,
            Func<TNode, bool> predicate)
            where TNode : SyntaxNode
        {
            SyntaxTree? tree = CSharpSyntaxTree.ParseText(targetCode);

            TNode target = tree.GetRoot()
                .ChildNodes()
                .GetNodesOfTypeRecursively<TNode>()
                .Single(predicate);

            Assert.Equal(expected, this.Sut.Match(target));
        }
    }
}
