using System;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class NestedBlockParentSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly NestedBlockParentSyntaxNodeMatcher instance =
            new NestedBlockParentSyntaxNodeMatcher();

        private NestedBlockParentSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return MethodBodyMemberSyntaxNodeMatcher.Instance.Match(node)
                && !MethodMemberSyntaxNodeMatcher.Instance.Match(node)
                && !MethodBodySyntaxNodeMatcher.Instance.Match(node);
        }

        public static NestedBlockParentSyntaxNodeMatcher Instance => instance;
    }
}
