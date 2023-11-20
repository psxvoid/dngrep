using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.Syntax
{
    public class AutoPropertyDeclarationSyntax : IVirtualSyntaxNode
    {
        public AutoPropertyDeclarationSyntax(PropertyDeclarationSyntax node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            this.Property = node;
        }

        public PropertyDeclarationSyntax Property { get; }

        public VirtualSyntaxNodeKind Kind => VirtualSyntaxNodeKind.AutoProperty;

        public SyntaxNode BaseNode => this.Property;

        public bool HasInitializer => this.Property.Initializer != null;
    }
}
