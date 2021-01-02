using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Routings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries.SyntaxWalkers
{
    public class BasicSyntaxTreeQueryWalker : CSharpSyntaxWalker
    {
        private readonly List<CombinedSyntaxNode> accumulator = new List<CombinedSyntaxNode>();

        private readonly IReadOnlyCollection<ISyntaxNodeMatcher> matchers;
        private readonly IReadOnlyCollection<IVirtualSyntaxNodeMatcher> virtualMatchers;
        private readonly IVirtualQueryRouting virtualQuery;

        public CombinedSyntaxNode[] Results => this.accumulator.ToArray();

        public BasicSyntaxTreeQueryWalker(
            BasicSyntaxTreeQuery query,
            IVirtualQueryRouting virtualQuery)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));
            _ = virtualQuery ?? throw new ArgumentNullException(nameof(virtualQuery));

            this.matchers = query.Matchers;
            this.virtualMatchers = query.VirtualMatchers;

            this.virtualQuery = virtualQuery;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            IVirtualSyntaxNode virtualNode = this.virtualQuery.Query(node);

            if (virtualNode.Kind != VirtualSyntaxNodeKind.Empty
                && this.virtualMatchers.All(x => x.Match(virtualNode)))
            {
                this.accumulator.Add(new CombinedSyntaxNode(virtualNode));
            }

            if(virtualNode.Kind == VirtualSyntaxNodeKind.Empty
                && this.matchers.All(x => x.Match(node)))
            {
                this.accumulator.Add(new CombinedSyntaxNode(node));
            }
            
            base.DefaultVisit(node);
        }
    }
}
