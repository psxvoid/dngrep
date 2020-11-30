using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public interface IVirtualSyntaxNode
    {
        VirtualSyntaxNodeKind Kind { get; }

        SyntaxNode BaseNode { get; }
    }
}
