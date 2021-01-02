using System;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class IfConditionVirtualQuery :
        IVirtualNodeQuery,
        ICanOverride<MethodBodyVirtualQuery>,
        ICanOverride<NestedBlockVirtualQuery>
    {
        private static readonly IfConditionVirtualQuery InstancePrivate
            = new IfConditionVirtualQuery();

        private IfConditionVirtualQuery()
        {
        }

        public bool HasOverride => true;

        public bool CanQuery(SyntaxNode node)
        {
            return node is ExpressionSyntax && node.Parent is IfStatementSyntax;
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (!(node is ExpressionSyntax expression) || !this.CanQuery(node))
            {
                throw new ArgumentException(
                    $"The provided node cannot be queried by {nameof(IfConditionVirtualQuery)}. " +
                    $"Actual node type is {node.GetType()}.");
            }

            return new IfConditionSyntax(expression);
        }

        public static IfConditionVirtualQuery Instance => InstancePrivate;
    }
}
