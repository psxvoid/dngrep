using System;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class IfBodyVirtualQuery :
        IVirtualNodeQuery,
        ICanOverride<MethodBodyVirtualQuery>,
        ICanOverride<NestedBlockVirtualQuery>
    {
        private static readonly IfBodyVirtualQuery InstancePrivate
            = new IfBodyVirtualQuery();

        private IfBodyVirtualQuery()
        {
        }

        public bool HasOverride => true;

        public bool CanQuery(SyntaxNode node)
        {
            return node is StatementSyntax && node.Parent is IfStatementSyntax;
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (!(node is StatementSyntax statement) || !this.CanQuery(node))
            {
                throw new ArgumentException(
                    $"The provided node cannot be queried by {nameof(IfBodyVirtualQuery)}. " +
                    $"Actual node type is {node.GetType()}.");
            }

            return new IfBodySyntax(statement);
        }

        public static IfBodyVirtualQuery Instance => InstancePrivate;
    }
}
