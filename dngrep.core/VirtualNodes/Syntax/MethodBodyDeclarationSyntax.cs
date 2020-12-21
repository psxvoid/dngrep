using System;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public sealed class MethodBodyDeclarationSyntax : IVirtualSyntaxNode
    {
        public enum BodyType
        {
            BlockBody,
            ArrowExpressionBody,
            ExpressionBody
        }

        private readonly BodyType bodyType;

        public BodyType ActiveBodyType => this.bodyType;

        public BlockSyntax? BlockBody { get; }

        public ArrowExpressionClauseSyntax? ArrowExpressionBody { get; }

        public ExpressionSyntax? ExpressionBody { get; }

        public MethodBodyDeclarationSyntax(SyntaxNode methodBody)
        {
            _ = methodBody ?? throw new ArgumentNullException(nameof(methodBody));

            if (!MethodBodySyntaxNodeMatcher.Instance.Match(methodBody))
            {
                throw new ArgumentException("The provided node isn't a method body.");
            }

            if (methodBody is BlockSyntax blockBody)
            {
                this.BlockBody = blockBody;
                this.bodyType = BodyType.BlockBody;
            }
            else if (methodBody is ArrowExpressionClauseSyntax arrowExpressionBody)
            {
                this.ArrowExpressionBody = arrowExpressionBody;
                this.bodyType = BodyType.ArrowExpressionBody;
            }
            else if (methodBody is ExpressionSyntax expressionBody)
            {
                this.ExpressionBody = expressionBody;
                this.bodyType = BodyType.ExpressionBody;
            }
            else
            {
                throw new ArgumentException(
                    $"Unknown method body for {nameof(MethodBodyDeclarationSyntax)}.");
            }
        }

        SyntaxNode IVirtualSyntaxNode.BaseNode =>
            (SyntaxNode?)this.BlockBody ??
            (SyntaxNode?)this.ArrowExpressionBody ??
            (SyntaxNode?)this.ExpressionBody
            ?? throw new NullReferenceException(
                $"The base node is not initialized for {nameof(MethodBodyDeclarationSyntax)}.");

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.MethodBody;
    }
}
