using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries
{
    public interface IVirtualNodeQuery
    {
        bool CanQuery(SyntaxNode node);

        IVirtualSyntaxNode Query(SyntaxNode node);

        bool HasOverride { get; }
    }
}
