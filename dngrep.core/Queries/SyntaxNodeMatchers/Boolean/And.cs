using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Boolean
{
    public class And : ISyntaxNodeMatcher
    {
        public ISyntaxNodeMatcher Lhs { get; }
        public ISyntaxNodeMatcher Rhs { get; }

        public And(ISyntaxNodeMatcher lhs, ISyntaxNodeMatcher rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public bool Match(SyntaxNode node)
        {
            return this.Lhs.Match(node) && this.Rhs.Match(node);
        }
    }
}
