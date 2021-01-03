using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using Microsoft.CodeAnalysis;
using static dngrep.core.Queries.INonOverridableVirtualNodeQuery;

namespace dngrep.core.VirtualNodes
{
    public class BasicVirtualQueryRouting : IVirtualQueryRouting
    {
        private readonly IVirtualQueryOverrideRouting overrideRouting;
        private readonly IReadOnlyCollection<IVirtualNodeQuery> queries;

        public BasicVirtualQueryRouting(
            IVirtualQueryOverrideRouting overrideRouting,
            IReadOnlyCollection<IVirtualNodeQuery> queries)
        {
            this.overrideRouting = overrideRouting;
            this.queries = queries;
        }

        public (InsertOrderType?, IVirtualSyntaxNode)[] Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            var results = new List<(InsertOrderType?, IVirtualSyntaxNode)>();

            IEnumerable<IVirtualNodeQuery> overridableQueries = this.queries
                .Where(x => !(x is INonOverridableVirtualNodeQuery) && x.CanQuery(node));

            IEnumerable<INonOverridableVirtualNodeQuery> nonOverridableQueries = this.queries
                .Where(x => x is INonOverridableVirtualNodeQuery && x.CanQuery(node))
                .Cast<INonOverridableVirtualNodeQuery>();

            foreach(INonOverridableVirtualNodeQuery query in nonOverridableQueries)
            {
                 results.Add((query.InsertOrder, query.Query(node)));
            }

            IVirtualNodeQuery? overridableQuery = null;

            int overridableQueryCount = overridableQueries.Count();

            if (overridableQueryCount > 1)
            {
                overridableQuery = this.overrideRouting.GetSingleOverride(overridableQueries);
            }
            else if (overridableQueryCount == 1)
            {
                overridableQuery = overridableQueries.Single();
            }

            if (overridableQuery != null)
            {
                results.Add((null, overridableQuery.Query(node)));
            }

            return results.ToArray();
        }
    }
}
