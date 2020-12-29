using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class IfConditionSyntax : IVirtualSyntaxNode
    {
        public ExpressionSyntax Expression { get; }

        public IfConditionSyntax(ExpressionSyntax expression)
        {
            this.Expression = expression;
        }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.IfCondition;

        public SyntaxNode BaseNode => this.Expression;
    }
}
