using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.VirtualNodes
{
    public interface IVirtualSyntaxNode
    {
        VirtualSyntaxNodeKind Kind { get; }

        SyntaxNode BaseNode { get; }
    }

    public static class VirtualSyntaxNode
    {
        private readonly static EmptySyntaxNode EmptyInstance = new EmptySyntaxNode();

        private class EmptySyntaxNode : IVirtualSyntaxNode
        {
            private readonly static EmptyStatementSyntax EmptyNode =
                SyntaxFactory.EmptyStatement();

            public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.Empty;

            public SyntaxNode BaseNode => EmptyNode;
        }

        public static IVirtualSyntaxNode Empty => EmptyInstance;
    }


    public interface IVirtualSyntaxNodeWithSpanOverride : IVirtualSyntaxNode
    {
        TextSpan SourceSpan { get; }
    }
}
