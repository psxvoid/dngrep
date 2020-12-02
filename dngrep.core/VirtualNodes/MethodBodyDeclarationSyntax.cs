using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public sealed class MethodBodyDeclarationSyntax : IVirtualSyntaxNode
    {
        public SyntaxNode BlockSyntax { get; }

        public MethodBodyDeclarationSyntax(SyntaxNode methodBody)
        {
            this.BlockSyntax = methodBody;
        }

        SyntaxNode IVirtualSyntaxNode.BaseNode => this.BlockSyntax;

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.MethodBody;
    }
}
