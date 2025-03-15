using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public interface IVirtualQueryRoutingFactory
    {
        IVirtualQueryRouting<T> Create<T>(
            Action<T> push,
            Func<T> pop,
            ISyntaxNodeMatchStrategy matchStrategy,
            Func<IVirtualSyntaxNode, T> mapResult,
            IVirtualQueryOverrideRouting overrideRouting);
    }

    public interface IVirtualQueryRouting<T>
    {
        public void QueryAndUpdateResults(
            IEnumerable<IVirtualNodeQuery> queries,
            SyntaxNode queryTarget);
    }

    public class VirtualQueryRoutingFactory : IVirtualQueryRoutingFactory
    {
        public IVirtualQueryRouting<T> Create<T>(
            Action<T> push,
            Func<T> pop,
            ISyntaxNodeMatchStrategy matchStrategy,
            Func<IVirtualSyntaxNode, T> mapResult,
            IVirtualQueryOverrideRouting overrideRouting)
        {
            return new VirtualQueryRouting<T>(
                push,
                pop,
                matchStrategy,
                mapResult,
                overrideRouting);
        }
    }

    public class VirtualQueryRouting<T> : IVirtualQueryRouting<T>
    {
        private readonly Action<T> push;
        private readonly Func<T> pop;
        private readonly ISyntaxNodeMatchStrategy matchStrategy;
        private readonly Func<IVirtualSyntaxNode, T> mapResult;
        private readonly IVirtualQueryOverrideRouting overrideRouting;

        public VirtualQueryRouting(
            Action<T> push,
            Func<T> pop,
            ISyntaxNodeMatchStrategy matchStrategy,
            Func<IVirtualSyntaxNode, T> resultMapper,
            IVirtualQueryOverrideRouting overrideRouting)
        {
            this.push = push;
            this.pop = pop;
            this.matchStrategy = matchStrategy;
            this.mapResult = resultMapper;
            this.overrideRouting = overrideRouting;
        }

        public void QueryAndUpdateResults(
            IEnumerable<IVirtualNodeQuery> queries,
            SyntaxNode queryTarget)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));
            _ = queryTarget ?? throw new ArgumentNullException(nameof(queryTarget));

            IEnumerable<IVirtualNodeQuery> overridableQueries = queries
                .Where(x => x.HasOverride && x.CanQuery(queryTarget));

            IEnumerable<IVirtualNodeQuery> nonOverridableQueries = queries
                .Where(x => !x.HasOverride && x.CanQuery(queryTarget));

            bool hasOverridableQueries = overridableQueries.Any();
            bool hasNonOverridableQueries = nonOverridableQueries.Any();

            if (hasOverridableQueries && hasNonOverridableQueries)
            {
                throw new InvalidOperationException(
                    "Virtual queries have an override conflict. " +
                    $"The query target type is {queryTarget.GetType()}.");
            }

            var overrideQuery = new IVirtualNodeQuery[1];

            if (hasOverridableQueries && queries.Count() > 1)
            {
                overrideQuery[0] = this.overrideRouting.GetSingleOverride(overridableQueries);
            }
            else if (hasOverridableQueries)
            {
                overrideQuery[0] = overridableQueries.Single();
            }

            IEnumerable<IVirtualNodeQuery> targetQueries = hasOverridableQueries
                ? overrideQuery
                : nonOverridableQueries;

            foreach (IVirtualNodeQuery query in targetQueries)
            {
                IVirtualSyntaxNode queryResult = query.Query(queryTarget);

                if (!this.matchStrategy.Match(queryResult.BaseNode)) return;

                if (query.HasOverride)
                {
                    this.pop();
                }

                this.push(this.mapResult(queryResult));
            }
        }
    }
}
