using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxWalkers.MatchStrategies
{
    public interface ISyntaxNodeMatchStrategy
    {
        bool Match(SyntaxNode node);
    }
}
