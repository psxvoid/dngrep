using System;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class TryBodyVirtualQuery : IVirtualNodeQuery, ICanOverride<MethodBodyVirtualQuery>
    {
        public bool HasOverride => true;

        public bool CanQuery(SyntaxNode node)
        {
            return node is BlockSyntax && node.Parent is TryStatementSyntax;
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (!(node is BlockSyntax block) || !this.CanQuery(node))
            {
                throw new ArgumentException(
                    $"The provided cannot be queried by {nameof(TryBodyVirtualQuery)}. " +
                    $"Actual node type is {node.GetType()}.");
            }

            return new TryBodySyntax(block);
        }
    }
}
