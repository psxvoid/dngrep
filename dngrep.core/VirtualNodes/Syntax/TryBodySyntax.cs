using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class TryBodySyntax : IVirtualSyntaxNode
    {
        public BlockSyntax Body { get; }

        public TryBodySyntax(BlockSyntax tryBody)
        {
            _ = tryBody ?? throw new ArgumentNullException(nameof(tryBody));

            if (tryBody.Parent?.GetType() != typeof(TryStatementSyntax))
            {
                throw new ArgumentException(
                    "The provided block is not a try statement body.",
                    nameof(tryBody));
            }

            this.Body = tryBody;
        }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.TryBody;

        public SyntaxNode BaseNode => this.Body;
    }
}
