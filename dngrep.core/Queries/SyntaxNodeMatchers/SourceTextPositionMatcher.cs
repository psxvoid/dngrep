using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class SourceTextPositionMatcher : ISyntaxNodeMatcher
    {
        private readonly TextSpan spanToMatch;

        public SourceTextPositionMatcher(TextSpan spanToMatch)
        {
            this.spanToMatch = spanToMatch;
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return node.GetLocation().SourceSpan.Contains(this.spanToMatch);
        }
    }
}
