using System;
using System.Collections.Generic;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public interface IVirtualQueryRoutingFactory
    {
        IVirtualQueryRouting<T> Create<T>(
            Action<T> push,
            Func<T> pop,
            ISyntaxNodeMatchStrategy matchStrategy,
            Func<IVirtualSyntaxNode, T> mapResult);
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
            Func<IVirtualSyntaxNode, T> mapResult)
        {
            return new VirtualQueryRouting<T>(
                push,
                pop,
                matchStrategy,
                mapResult);
        }
    }

    public class VirtualQueryRouting<T> : IVirtualQueryRouting<T>
    {
        private readonly Action<T> push;
        private readonly Func<T> pop;
        private readonly ISyntaxNodeMatchStrategy matchStrategy;
        private readonly Func<IVirtualSyntaxNode, T> mapResult;

        public VirtualQueryRouting(
            Action<T> push,
            Func<T> pop,
            ISyntaxNodeMatchStrategy matchStrategy,
            Func<IVirtualSyntaxNode, T> resultMapper)
        {
            this.push = push;
            this.pop = pop;
            this.matchStrategy = matchStrategy;
            this.mapResult = resultMapper;
        }

        public void QueryAndUpdateResults(
            IEnumerable<IVirtualNodeQuery> queries,
            SyntaxNode queryTarget)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));
            _ = queryTarget ?? throw new ArgumentNullException(nameof(queryTarget));

            bool hasOverride = false;
            bool hasSkippedOverride = false;
            bool overrideOccurred = false;

            foreach (IVirtualNodeQuery query in queries)
            {
                IVirtualSyntaxNode queryResult = query.Query(queryTarget);

                if (!this.matchStrategy.Match(queryResult.BaseNode)) continue;

                if (query.HasOverride)
                {
                    hasOverride = true;
                    if (!overrideOccurred)
                    {
                        this.pop();
                        overrideOccurred = true;
                    }
                }
                else
                {
                    hasSkippedOverride = true;
                }

                if (hasOverride && hasSkippedOverride)
                {
                    throw new InvalidOperationException(
                        "Virtual queries have result override conflict.");
                }

                this.push(this.mapResult(queryResult));
            }
        }
    }
}
