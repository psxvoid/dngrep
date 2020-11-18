using System;
using System.Linq;
using dngrep.core.Queries.Specifiers;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class KnownTargetMatcher : ISyntaxNodeMatcher
    {
        private static readonly KnownTargetMatcher CachedInstance = new KnownTargetMatcher();

        private KnownTargetMatcher()
        {
        }

        public static KnownTargetMatcher Instance => CachedInstance;

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return KnownQueryTargets.EqualSyntaxNodeTypes.Contains(node.GetType());
        }
    }
}
