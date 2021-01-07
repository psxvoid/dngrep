﻿using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class MethodBodyMemberSyntaxNodeMatcher : ISyntaxNodeMatcher, ICombinedSyntaxNodeMatcher
    {
        private static readonly MethodBodyMemberSyntaxNodeMatcher instance =
            new MethodBodyMemberSyntaxNodeMatcher();

        public static MethodBodyMemberSyntaxNodeMatcher Instance => instance;

        private readonly static Type[] NonVirtualBodyMemberSiblings = new[]
        {
            typeof(IfStatementSyntax),
            typeof(LocalDeclarationStatementSyntax),
            typeof(LocalFunctionStatementSyntax),
            typeof(ReturnStatementSyntax),
            typeof(ForStatementSyntax),
            typeof(ForEachStatementSyntax),
            typeof(WhileStatementSyntax),
            typeof(BlockSyntax),
            typeof(ArrowExpressionClauseSyntax),
            typeof(ExpressionStatementSyntax),
            typeof(TryStatementSyntax),
            typeof(ThrowStatementSyntax),
        };

        private readonly static Type[] BlockSyntaxParentExcludes = new[]
        {
            typeof(MethodDeclarationSyntax),
            typeof(IfStatementSyntax),
            typeof(ElseClauseSyntax),
            typeof(WhileStatementSyntax),
            typeof(ForStatementSyntax),
            typeof(ForEachStatementSyntax),
            typeof(TryStatementSyntax),
            typeof(FinallyClauseSyntax),
            typeof(CatchClauseSyntax),
            typeof(LocalFunctionStatementSyntax),
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

            if (!NonVirtualBodyMemberSiblings.Contains(node.GetType()))
            {
                return false;
            }

            if (node is BlockSyntax block
                && block.Parent != null
                && BlockSyntaxParentExcludes.Contains(block.Parent.GetType()))
            {
                return false;
            }

            return IsNotTrivialStatement(node);
        }

        public bool Match(CombinedSyntaxNode node)
        {
            return CombinedMethodBodyMembers.Contains(node.MixedNode.GetType())
                || (node.BaseNode is ExpressionSyntax && !(node.BaseNode is PredefinedTypeSyntax))
                || IsNotTrivialStatement(node.BaseNode);
        }

        private static bool IsNotTrivialStatement(SyntaxNode? node)
        {
            if (node == null)
            {
                return false;
            }

            return node is StatementSyntax
                && node.GetType() != typeof(EmptyStatementSyntax);
        }
    }
}
