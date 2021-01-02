using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes.Routings
{
    public interface IVirtualQueryRouting
    {
        IVirtualSyntaxNode Query(SyntaxNode node);
    }
}
