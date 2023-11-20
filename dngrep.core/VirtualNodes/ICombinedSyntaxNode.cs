using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public interface ICombinedSyntaxNode
    {
        bool IsVirtual { get; }

        SyntaxNode BaseNode { get; }
    }
}
