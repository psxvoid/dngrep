using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxWalkers
{
    public class CombinedSyntaxTreeQueryWalker : SyntaxTreeQueryWalkerBase<CombinedSyntaxNode>
    {
        private readonly IVirtualQueryRoutingFactory queryRoutingFactory;
        private readonly ISyntaxNodeMatchStrategy matchStrategy;
        private readonly VirtualQueryOverrideRouting overrideRouting;

        public IReadOnlyCollection<IVirtualNodeQuery> VirtualQueries { get; }

        public CombinedSyntaxTreeQueryWalker(
            CombinedSyntaxTreeQuery query,
            IVirtualQueryRoutingFactory queryRoutingFactory,
            VirtualQueryOverrideRouting overrideRouting,
            ISyntaxNodeMatchStrategy? baseMatchStrategy = null,
            ISyntaxNodeMatchStrategy? matchStrategy = null
            ) : base(query, baseMatchStrategy)
        {
            this.VirtualQueries = query.VirtualNodeQueries;
            this.queryRoutingFactory = queryRoutingFactory;
            this.matchStrategy = matchStrategy ?? base.BaseStrategy;
            this.overrideRouting = overrideRouting;
        }

        protected override CombinedSyntaxNode CreateResultFromNode(SyntaxNode node)
        {
            return new CombinedSyntaxNode(node);
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            int nodesBefore = this.Results.Count;

            base.BaseVisit(node);

            int nodesAfter = this.Results.Count;

            if (this.VirtualQueries.IsNullOrEmpty())
            {
                base.DefaultVisit(node);

                return;
            }

            if (nodesAfter == nodesBefore)
            {
                base.DefaultVisit(node);

                return;
            }

            SyntaxNode target = this.PeekResult().BaseNode;

            IEnumerable<IVirtualNodeQuery> queries = this.VirtualQueries.Where(
                x => x.CanQuery(target));

            if (queries.IsNullOrEmpty())
            {
                base.DefaultVisit(node);

                return;
            }

            IVirtualQueryRouting<CombinedSyntaxNode> queryRouting =
                this.queryRoutingFactory.Create(
                    this.PushResult,
                    this.PopResult,
                    this.matchStrategy,
                    x => new CombinedSyntaxNode(x),
                    this.overrideRouting);

            queryRouting.QueryAndUpdateResults(queries, target);

            base.DefaultVisit(node);
        }
    }
}
