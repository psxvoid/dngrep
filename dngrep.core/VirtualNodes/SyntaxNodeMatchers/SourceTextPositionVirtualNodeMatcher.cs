using System;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.VirtualNodes.SyntaxNodeMatchers
{
    public class SourceTextPositionVirtualNodeMatcher : IVirtualSyntaxNodeMatcher
    {
        public TextSpan SpanToMatch { get; }

        public SourceTextPositionVirtualNodeMatcher(TextSpan spanToMatch)
        {
            this.SpanToMatch = spanToMatch;
        }

        public bool Match(IVirtualSyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (node is IVirtualSyntaxNodeWithSpanOverride nodeWithSpanOverride)
            {
                return nodeWithSpanOverride.SourceSpan.Contains(this.SpanToMatch);
            }

            return node.BaseNode.GetLocation().SourceSpan.Contains(this.SpanToMatch);
        }
    }
}
