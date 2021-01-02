using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using Microsoft.CodeAnalysis;

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

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            IEnumerable<IVirtualNodeQuery> overridableQueries = this.queries
                .Where(x => x.HasOverride && x.CanQuery(node));

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
                return VirtualSyntaxNode.Empty;
            }

            return query.Query(node);
        }
    }
}
