using System;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public struct CombinedSyntaxNode : ICombinedSyntaxNode, IEquatable<CombinedSyntaxNode>
    {
        public CombinedSyntaxNode(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            this.Node = node;
            this.VirtualNode = null;
        }

        public CombinedSyntaxNode(IVirtualSyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            this.Node = null;
            this.VirtualNode = node;
        }

        public SyntaxNode? Node { get; }

        public IVirtualSyntaxNode? VirtualNode { get; }

        public bool IsVirtual => this.VirtualNode != null;

        public SyntaxNode BaseNode
        {
            get
            {
                SyntaxNode? node = this.IsVirtual
                    ? this.VirtualNode?.BaseNode
                    : this.Node;

                _ = node ?? throw new InvalidOperationException("The base node is not defined.");

                return node;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(CombinedSyntaxNode))
            {
                return this.Equals((CombinedSyntaxNode)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                hash = hash * 23 + (!this.IsVirtual ? this.Node.GetHashCode() : 0);
                hash = hash * 23 + (this.IsVirtual ? this.VirtualNode.GetHashCode() : 0);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            return hash;
        }

        public static bool operator ==(CombinedSyntaxNode left, CombinedSyntaxNode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CombinedSyntaxNode left, CombinedSyntaxNode right)
        {
            return !(left == right);
        }

        public bool Equals(CombinedSyntaxNode other)
        {
            return other != null
                && this.IsVirtual == other.IsVirtual
                && this.BaseNode == other.BaseNode;
        }
    }
}
