using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries
{
    public interface ISyntaxNodeMatcher
    {
        bool Match(SyntaxNode node);
    }
}
