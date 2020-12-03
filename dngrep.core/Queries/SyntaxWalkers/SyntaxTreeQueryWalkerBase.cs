using System;
using System.Collections.Generic;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries.SyntaxWalkers
{
    public abstract class SyntaxTreeQueryWalkerBase<T> : CSharpSyntaxWalker
    {
        private readonly List<T> results = new List<T>();
        private readonly ISyntaxNodeMatchStrategy matchStrategy;

        protected abstract T CreateResultFromNode(SyntaxNode node);
        protected ISyntaxNodeMatchStrategy BaseStrategy => this.matchStrategy;

        public IReadOnlyCollection<T> Results => this.results;

        protected T PeekResult()
        {
            if (this.results.Count == 0)
            {
                throw new InvalidOperationException("Unable to peek from an empty list.");
            }

            return this.results[this.results.Count - 1];
        }

        protected T PopResult()
        {
            if (this.results.Count == 0)
            {
                throw new InvalidOperationException("Unable to pop from an empty list.");
            }

            int index = this.results.Count - 1;

            T last = this.results[index];
            this.results.RemoveAt(index);
            return last;
        }

        protected void PushResult(T result)
        {
            this.results.Add(result);
        }

        public SyntaxTreeQueryWalkerBase(
            SyntaxTreeQuery query,
            ISyntaxNodeMatchStrategy? matchStrategy = null
            )
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            this.matchStrategy = matchStrategy ?? new ScopedSyntaxNodeMatchStrategy(query);
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            if (this.matchStrategy.Match(node))
            {
                this.PushResult(this.CreateResultFromNode(node));
            }

            base.DefaultVisit(node);
        }
    }
}
