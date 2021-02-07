using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class ElseBodySyntax : IVirtualSyntaxNode
    {
        public BlockSyntax Body { get; }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.ElseBody;

        public SyntaxNode BaseNode => this.Body;

        public ElseBodySyntax(BlockSyntax body)
        {
            this.Body = body;
        }
    }
}
