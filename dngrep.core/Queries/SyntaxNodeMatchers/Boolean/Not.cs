using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class Not : ISyntaxNodeMatcher
    {
        public ISyntaxNodeMatcher Matcher { get; }

        public Not(ISyntaxNodeMatcher matcher)
        {
            this.Matcher = matcher;
        }

        public bool Match(SyntaxNode node)
        {
            return !this.Matcher.Match(node);
        }
    }
}
