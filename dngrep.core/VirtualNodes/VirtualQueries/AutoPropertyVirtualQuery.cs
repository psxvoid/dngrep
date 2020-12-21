using System;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class AutoPropertyVirtualQuery : IVirtualNodeQuery
    {
        private static readonly AutoPropertyVirtualQuery InstancePrivate =
            new AutoPropertyVirtualQuery();

        public static AutoPropertyVirtualQuery Instance => InstancePrivate;

        private AutoPropertyVirtualQuery()
        {
        }

        public bool HasOverride => true;

        public bool CanQuery(SyntaxNode node)
        {
            if (node is PropertyDeclarationSyntax prop)
            {
                return prop.ExpressionBody == null
                    && prop.AccessorList != null
                        && prop.AccessorList.Accessors.All(
                            x => x.Body == null && x.ExpressionBody == null);
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

            return new AutoPropertyDeclarationSyntax(prop);
        }
    }
}
