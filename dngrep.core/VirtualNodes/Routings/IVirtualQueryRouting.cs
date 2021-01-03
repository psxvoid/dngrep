using Microsoft.CodeAnalysis;
using static dngrep.core.Queries.INonOverridableVirtualNodeQuery;

namespace dngrep.core.VirtualNodes.Routings
{
    public interface IVirtualQueryRouting
    {
        (InsertOrderType?, IVirtualSyntaxNode) Query(SyntaxNode node);
    }
}
