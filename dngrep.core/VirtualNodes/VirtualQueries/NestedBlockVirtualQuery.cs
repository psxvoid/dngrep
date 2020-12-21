using System;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class NestedBlockVirtualQuery : IVirtualNodeQuery
    {
        private static readonly NestedBlockVirtualQuery InstancePrivate
            = new NestedBlockVirtualQuery();

        public static NestedBlockVirtualQuery Instance => InstancePrivate;

        public bool HasOverride => true;

        private NestedBlockVirtualQuery()
        {
        }

        public bool CanQuery(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return NestedBlockSyntaxNodeMatcher.Instance.Match(node);
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            SyntaxNode? parent = node.Parent;

            if (parent == null)
            {
                throw new ArgumentException(
                    "The provided node does not have a parent." +
                    "It is required to properly query the nested block.",
                    nameof(node));
            }

            if (!NestedBlockParentSyntaxNodeMatcher.Instance.Match(parent))
            {
                throw new InvalidOperationException(
                    "The provided node is not a nested block. " +
                    "Ensure that it is not a direct body of a constructor, method, etc.");
            }

            return new NestedBlockSyntax(node);
        }
    }
}
