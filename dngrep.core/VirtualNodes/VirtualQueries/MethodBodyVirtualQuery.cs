using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class MethodBodyVirtualQuery : IVirtualNodeQuery
    {
        private static readonly MethodBodyVirtualQuery InstancePrivate
            = new MethodBodyVirtualQuery();

        public static MethodBodyVirtualQuery Instance => InstancePrivate;

        public bool HasOverride => true;

        private MethodBodyVirtualQuery()
        {
        }

        public bool CanQuery(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return MethodBodySyntaxNodeMatcher.Instance.Match(node);
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            SyntaxNode? parent = node.Parent;

            if (parent == null)
            {
                throw new ArgumentException(
                    "The provided node does not have a parent." +
                    "It is required to properly query the method body.",
                    nameof(node));
            }

            if (!MethodBodyParentSyntaxNodeMatcher.Instance.Match(parent))
            {
                throw new InvalidOperationException(
                    "The provided node does not have a method body.");
            }

            
            return new MethodBodyDeclarationSyntax(parent.GetBody());
        }
    }
}
