using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class NestedBlockParentSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly NestedBlockParentSyntaxNodeMatcher instance =
            new NestedBlockParentSyntaxNodeMatcher();

        private NestedBlockParentSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            // ExpressionStatement and ArrowExpressionClause cannot contain nested blocks
            return // !(node is ExpressionStatementSyntax)
                // && !(node is ArrowExpressionClauseSyntax)
                (MethodBodyMemberSyntaxNodeMatcher.Instance.Match(node)
                    || MethodBodySyntaxNodeMatcher.Instance.Match(node)
                    || node.GetType() == typeof(FinallyClauseSyntax)
                    || node.GetType() == typeof(CatchClauseSyntax)
                    || node.GetType() == typeof(ElseClauseSyntax)
                    || node is ExpressionSyntax
                    || node is BlockSyntax)
                && node.GetType() != typeof(LocalFunctionStatementSyntax);
        }

        public static NestedBlockParentSyntaxNodeMatcher Instance => instance;
    }
}
