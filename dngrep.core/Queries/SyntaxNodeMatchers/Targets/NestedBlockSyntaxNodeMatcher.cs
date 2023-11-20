using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class NestedBlockSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly NestedBlockSyntaxNodeMatcher instance =
            new NestedBlockSyntaxNodeMatcher();

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (node.Parent != null
                && NestedBlockParentSyntaxNodeMatcher.Instance.Match(node.Parent)
                && node.IsContainer()
                && !MethodBodySyntaxNodeMatcher.Instance.Match(node))
            {
                return true;
            }

            return false;
        }

        public static NestedBlockSyntaxNodeMatcher Instance => instance;
    }
}
