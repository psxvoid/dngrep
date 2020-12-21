using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class MethodMemberSyntaxNodeMatcher : ICombinedSyntaxNodeMatcher
    {
        private static readonly MethodMemberSyntaxNodeMatcher instance =
            new MethodMemberSyntaxNodeMatcher();

        private readonly static Type[] NonVirtualMemberTypes = new[]
        {
            typeof(ParameterListSyntax),
        };

        private readonly static Type[] VirtualMemberTypes = new[]
        {
            typeof(BlockSyntax),
            typeof(ArrowExpressionClauseSyntax),
            typeof(ExpressionSyntax),
        };

        private readonly static Type[] MixedMemberTypes = new[]
        {
            typeof(MethodBodyDeclarationSyntax),    // VirtualNode
            typeof(NestedBlockSyntax),              // VirtualNode
            typeof(ParameterListSyntax),            // Non-VirtualNode
        };

        private MethodMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();

            if (NonVirtualMemberTypes.Contains(nodeType))
            {
                return true;
            }

            bool isPotentialMethodBody = VirtualMemberTypes.Contains(nodeType);

            if (isPotentialMethodBody)
            {
                return this.Match(node.QueryVirtualAndCombine(MethodBodyVirtualQuery.Instance));
            }

            return false;
        }

        public bool Match(CombinedSyntaxNode node)
        {
            if (MixedMemberTypes.Contains(node.MixedNode.GetType()))
            {
                return true;
            }

            return false;
        }

        public static MethodMemberSyntaxNodeMatcher Instance => instance;
    }
}
