using System;
using System.Collections.Generic;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes
{
    public interface IVirtualQueryRoutingFactory
    {
        IVirtualQueryRouting<T> Create<T>(
            Action<T> push,
            Func<T> pop,
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
            Func<IVirtualSyntaxNode, T> mapResult)
        {
            return new VirtualQueryRouting<T>(
                push,
                pop,
                mapResult);
        }
    }

    public class VirtualQueryRouting<T> : IVirtualQueryRouting<T>
    {
        private readonly Action<T> push;
        private readonly Func<T> pop;
        private readonly Func<IVirtualSyntaxNode, T> mapResult;

        public VirtualQueryRouting(
            Action<T> push,
            Func<T> pop,
            Func<IVirtualSyntaxNode, T> resultMapper)
        {
            this.push = push;
            this.pop = pop;
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

            foreach (var query in queries)
            {
                IVirtualSyntaxNode queryResult = query.Query(queryTarget);

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
