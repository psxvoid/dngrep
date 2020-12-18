using System;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes
{
    public class NestedBlockSyntax : IVirtualSyntaxNode
    {
        public enum BlockType
        {
            BlockBody,
            ArrowExpressionBody,
            ExpressionBody
        }

        private readonly BlockType bodyType;

        public BlockType ActiveBodyType => this.bodyType;

        public BlockSyntax? BlockBody { get; }

        public ArrowExpressionClauseSyntax? ArrowExpressionBody { get; }

        public ExpressionSyntax? ExpressionBody { get; }

        public NestedBlockSyntax(SyntaxNode methodBody)
        {
            _ = methodBody ?? throw new ArgumentNullException(nameof(methodBody));

            if (!NestedBlockSyntaxNodeMatcher.Instance.Match(methodBody))
            {
                throw new ArgumentException("The provided node isn't a method body.");
            }

            if (methodBody is BlockSyntax blockBody)
            {
                this.BlockBody = blockBody;
                this.bodyType = BlockType.BlockBody;
            }
            else if (methodBody is ArrowExpressionClauseSyntax arrowExpressionBody)
            {
                this.ArrowExpressionBody = arrowExpressionBody;
                this.bodyType = BlockType.ArrowExpressionBody;
            }
            else if (methodBody is ExpressionSyntax expressionBody)
            {
                this.ExpressionBody = expressionBody;
                this.bodyType = BlockType.ExpressionBody;
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
