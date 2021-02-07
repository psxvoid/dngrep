using System;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class ElseBodyVirtualQuery :
        IVirtualNodeQuery,
        ICanOverride<MethodBodyVirtualQuery>,
        ICanOverride<NestedBlockVirtualQuery>
    {
        private static readonly ElseBodyVirtualQuery InstancePrivate
            = new ElseBodyVirtualQuery();

        private ElseBodyVirtualQuery()
        {
        }

        public bool HasOverride => true;

        public bool CanQuery(SyntaxNode node)
        {
            return node is BlockSyntax && node.Parent is ElseClauseSyntax;
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (!(node is BlockSyntax block) || !this.CanQuery(node))
            {
                throw new ArgumentException(
                    $"The provided node cannot be queried by {nameof(ElseBodyVirtualQuery)}. " +
                    $"Actual node type is {node.GetType()}.");
            }

            return new ElseBodySyntax(block);
        }

        public static ElseBodyVirtualQuery Instance => InstancePrivate;
    }
}
