using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class MethodBodySyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly MethodBodySyntaxNodeMatcher instance =
            new MethodBodySyntaxNodeMatcher();

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));
            if (node.Parent != null
                && MethodBodyParentSyntaxNodeMatcher.Instance.Match(node.Parent)
                && !(node is TypeSyntax)
                && (node is BlockSyntax
                    || node is ArrowExpressionClauseSyntax
                    || node is ExpressionSyntax))
            {
                return true;
            }

            return false;
        }

        public static MethodBodySyntaxNodeMatcher Instance => instance;
    }
}
