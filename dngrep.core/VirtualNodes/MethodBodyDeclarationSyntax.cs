using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes
{
    public sealed class MethodBodyDeclarationSyntax : IVirtualSyntaxNode
    {
        public BlockSyntax BlockSyntax { get; }

        public MethodBodyDeclarationSyntax(BlockSyntax blockSyntax)
        {
            this.BlockSyntax = blockSyntax;
        }

        SyntaxNode IVirtualSyntaxNode.BaseNode => this.BlockSyntax;

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.MethodBody;
    }
}
