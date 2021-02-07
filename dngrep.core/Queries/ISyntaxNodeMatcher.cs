using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries
{
    public interface ISyntaxNodeMatcher
    {
        bool Match(SyntaxNode node);
    }

    public interface ICombinedSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        bool Match(CombinedSyntaxNode node);
    }

    public interface IVirtualSyntaxNodeMatcher
    {
        bool Match(IVirtualSyntaxNode node);
    }
}
