using System;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class ReadOnlyPropertyVirtualQuery : IVirtualNodeQuery
    {
        private static readonly ReadOnlyPropertyVirtualQuery InstancePrivate =
            new ReadOnlyPropertyVirtualQuery();

        public static ReadOnlyPropertyVirtualQuery Instance => InstancePrivate;

        private ReadOnlyPropertyVirtualQuery()
        {
        }

        public bool HasOverride => true;

        public bool CanQuery(SyntaxNode node)
        {
            if (node is PropertyDeclarationSyntax prop)
            {
                return prop.AccessorList == null
                    && prop.ExpressionBody != null;
            }

            return false;
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (!this.CanQuery(node))
            {
                throw new ArgumentException("Unsupported node type.", nameof(node));
            }

            if (!(node is PropertyDeclarationSyntax prop))
            {
                throw new ArgumentException(
                    "Only PropertyDeclarationSyntax can be queried.", nameof(node));
            }

            return new ReadOnlyPropertyDeclarationSyntax(prop);
        }
    }
}
