using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class MethodBodyMemberSyntaxNodeMatcher : ISyntaxNodeMatcher, ICombinedSyntaxNodeMatcher
    {
        private static readonly MethodBodyMemberSyntaxNodeMatcher instance =
            new MethodBodyMemberSyntaxNodeMatcher();

        private readonly static Type[] NonVirtualBodyMemberSiblings = new[]
        {
            typeof(IfStatementSyntax),
            typeof(LocalDeclarationStatementSyntax),
            typeof(ReturnStatementSyntax),
            typeof(ForStatementSyntax),
            typeof(ForEachStatementSyntax),
            typeof(WhileStatementSyntax),
            typeof(BlockSyntax),
            typeof(ArrowExpressionClauseSyntax),
        };

        private readonly static Type[] VirtualBodyMemberSiblings = new[]
        {
            typeof(NestedBlockSyntax)
        };

        private readonly static Type[] CombinedMethodBodyMembers =
            NonVirtualBodyMemberSiblings.Concat(VirtualBodyMemberSiblings).ToArray();

        private MethodBodyMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return NonVirtualBodyMemberSiblings.Contains(node.GetType());
        }

        public bool Match(CombinedSyntaxNode node)
        {
            return CombinedMethodBodyMembers.Contains(node.MixedNode.GetType());
        }

        public static MethodBodyMemberSyntaxNodeMatcher Instance => instance;
    }
}
