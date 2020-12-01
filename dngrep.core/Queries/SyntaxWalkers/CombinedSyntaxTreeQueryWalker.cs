using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxWalkers
{
    public class CombinedSyntaxTreeQueryWalker : SyntaxTreeQueryWalkerBase<CombinedSyntaxNode>
    {
        private readonly IVirtualQueryRoutingFactory queryRoutingFactory;

        public IReadOnlyCollection<IVirtualNodeQuery> VirtualQueries { get; }

        public CombinedSyntaxTreeQueryWalker(
            CombinedSyntaxTreeQuery query,
            IVirtualQueryRoutingFactory queryRoutingFactory
            ) : base(query)
        {
            this.VirtualQueries = query.VirtualNodeQueries;
            this.queryRoutingFactory = queryRoutingFactory;
        }

        protected override CombinedSyntaxNode CreateResultFromNode(SyntaxNode node)
        {
            return new CombinedSyntaxNode(node);
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            int nodesBefore = this.Results.Count;

            base.DefaultVisit(node);

            if (this.VirtualQueries.IsNullOrEmpty())
            {
                return;
            }

            int nodesAfter = this.Results.Count;

            if (nodesAfter == nodesBefore)
            {
                return;
            }

            SyntaxNode queryTarget = this.PeekResult().Node
                ?? throw new InvalidOperationException("Query result cannot be empty.");

            IEnumerable<IVirtualNodeQuery> queries = this.VirtualQueries.Where(
                x => x.CanQuery(queryTarget));

            if (queries.IsNullOrEmpty())
            {
                return;
            }

            IVirtualQueryRouting<CombinedSyntaxNode>? queryRouting =
                this.queryRoutingFactory.Create(
                    this.PushResult,
                    this.PopResult,
                    x => new CombinedSyntaxNode(x));

            queryRouting.QueryAndUpdateResults(queries, queryTarget);
        }
    }
}
