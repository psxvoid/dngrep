using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class IfBodySyntax : IVirtualSyntaxNode
    {
        public StatementSyntax Statement { get; }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.IfBody;

        public SyntaxNode BaseNode => this.Statement;

        public IfBodySyntax(StatementSyntax statement)
        {
            this.Statement = statement;
        }
    }
}
