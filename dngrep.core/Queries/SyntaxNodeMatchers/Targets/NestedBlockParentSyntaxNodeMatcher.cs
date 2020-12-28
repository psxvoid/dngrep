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

            return !(node is ExpressionStatementSyntax)
                && (MethodBodyMemberSyntaxNodeMatcher.Instance.Match(node)
                    || node is TryStatementSyntax
                    || node is FinallyClauseSyntax
                    || node is CatchClauseSyntax
                    || node is ElseClauseSyntax)
                && !MethodMemberSyntaxNodeMatcher.Instance.Match(node)
                && !MethodBodySyntaxNodeMatcher.Instance.Match(node);
        }

        public static NestedBlockParentSyntaxNodeMatcher Instance => instance;
    }
}
