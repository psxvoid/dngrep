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

        public (InsertOrderType?, IVirtualSyntaxNode) Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            IEnumerable<IVirtualNodeQuery> overridableQueries = this.queries
                .Where(x => x.CanQuery(node));

            IVirtualNodeQuery query;

            int queryCount = overridableQueries.Count();

            if (queryCount > 1)
            {
                query = this.overrideRouting.GetSingleOverride(overridableQueries);
            }
            else if (queryCount == 1)
            {
                query = overridableQueries.Single();
            }
            else
            {
                return (null, VirtualSyntaxNode.Empty);
            }

            InsertOrderType? insertOrderType = null;

            if (query is INonOverridableVirtualNodeQuery nonOverridableQuery)
            {
                 insertOrderType = nonOverridableQuery.InsertOrder;
            }

            return (insertOrderType, query.Query(node));
        }
    }
}
