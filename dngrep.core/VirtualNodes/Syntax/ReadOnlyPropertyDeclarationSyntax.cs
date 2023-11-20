using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class ReadOnlyPropertyDeclarationSyntax : IVirtualSyntaxNode
    {
        public ReadOnlyPropertyDeclarationSyntax(PropertyDeclarationSyntax node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            this.Property = node;
        }

        public PropertyDeclarationSyntax Property { get; }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.ReadOnlyProperty;

        public SyntaxNode BaseNode => this.Property;
    }
}
