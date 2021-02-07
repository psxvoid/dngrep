using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Routings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static dngrep.core.Queries.INonOverridableVirtualNodeQuery;

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
            (InsertOrderType? insertOrder, IVirtualSyntaxNode virtualNode)[] results =
                this.virtualQuery.Query(node);

            var insertAfter = new List<IVirtualSyntaxNode>();
            int overrideCount = 0;
            IVirtualSyntaxNode? overridden = null;

            foreach ((InsertOrderType? insertOrder, IVirtualSyntaxNode virtualNode) in results)
            {
                bool hasOverride = insertOrder == null;

                bool hasVirtual = virtualNode.Kind != VirtualSyntaxNodeKind.Empty
                    && this.virtualMatchers.All(x => x.Match(virtualNode));
                
                overrideCount += hasOverride && hasVirtual ? 1 : 0;

                if (hasOverride && hasVirtual)
                {
                    overridden = virtualNode;
                }

                if (!hasOverride && hasVirtual && insertOrder == InsertOrderType.Before)
                {
                    this.accumulator.Add(new CombinedSyntaxNode(virtualNode));
                }

                if (!hasOverride && hasVirtual && insertOrder == InsertOrderType.After)
                {
                    insertAfter.Add(virtualNode);
                }
            }

            if (overrideCount > 1)
            {
                throw new InvalidOperationException(
                    $"Override conflict detected. Override count: {overrideCount}.");
            }

            if (overrideCount == 0 && this.matchers.All(x => x.Match(node)))
            {
                this.accumulator.Add(new CombinedSyntaxNode(node));
            } else if (overrideCount == 1 && overridden != null)
            {
                this.accumulator.Add(new CombinedSyntaxNode(overridden));
            }
            else if (overrideCount == 1)
            {
                throw new InvalidOperationException(
                    "Override detected but the substitute node is missing.");
            }

            if (insertAfter.Any())
            {
                this.accumulator.AddRange(insertAfter.Select(x => new CombinedSyntaxNode(x)));
            }

            base.DefaultVisit(node);
        }
    }
}
