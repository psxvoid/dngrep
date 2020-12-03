using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxWalkers
{
    public class CombinedSyntaxTreeQueryWalker : SyntaxTreeQueryWalkerBase<CombinedSyntaxNode>
    {
        private readonly IVirtualQueryRoutingFactory queryRoutingFactory;
        private readonly ISyntaxNodeMatchStrategy matchStrategy;

        public IReadOnlyCollection<IVirtualNodeQuery> VirtualQueries { get; }

        public CombinedSyntaxTreeQueryWalker(
            CombinedSyntaxTreeQuery query,
            IVirtualQueryRoutingFactory queryRoutingFactory,
            ISyntaxNodeMatchStrategy? matchStrategy = null
            ) : base(query)
        {
            this.VirtualQueries = query.VirtualNodeQueries;
            this.queryRoutingFactory = queryRoutingFactory;
            this.matchStrategy = matchStrategy ?? base.BaseStrategy;
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

            SyntaxNode target = this.PeekResult().BaseNode;

            IEnumerable<IVirtualNodeQuery> queries = this.VirtualQueries.Where(
                x => x.CanQuery(target));

            if (queries.IsNullOrEmpty())
            {
                return;
            }

            IVirtualQueryRouting<CombinedSyntaxNode> queryRouting =
                this.queryRoutingFactory.Create(
                    this.PushResult,
                    this.PopResult,
                    this.matchStrategy,
                    x => new CombinedSyntaxNode(x));

            queryRouting.QueryAndUpdateResults(queries, target);
        }
    }
}
