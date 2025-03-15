using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class IfConditionSyntax : IVirtualSyntaxNodeWithSpanOverride
    {
        public ExpressionSyntax Expression { get; }

        public IfConditionSyntax(ExpressionSyntax expression)
        {
            this.Expression = expression;
        }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.IfCondition;

        public SyntaxNode BaseNode => this.Expression;

        public TextSpan SourceSpan
        {
            get
            {
                if (!(this.Expression.Parent is IfStatementSyntax @if))
                {
                    throw new InvalidOperationException(
                        $"The parent node should be {nameof(IfStatementSyntax)}");
                }

                return TextSpan.FromBounds(
                    @if.OpenParenToken.SpanStart,
                    @if.CloseParenToken.SpanStart + 1);
            }
        }
    }
}
