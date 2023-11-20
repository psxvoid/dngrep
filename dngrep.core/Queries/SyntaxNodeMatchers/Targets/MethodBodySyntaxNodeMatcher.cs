using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

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
                && node.IsContainer())
            {
                return true;
            }

            return false;
        }

        public static MethodBodySyntaxNodeMatcher Instance => instance;
    }
}
