using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public abstract class SyntaxTreeQueryWalkerBase<T> : CSharpSyntaxWalker
    {
        private readonly List<T> results = new List<T>();
        private readonly SyntaxTreeQuery query;

        private SyntaxNode? scope;

        protected abstract T CreateResultFromNode(SyntaxNode node);

        public IReadOnlyCollection<T> Results => this.results;

        public SyntaxTreeQueryWalkerBase(SyntaxTreeQuery query)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            this.query = query;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();
            string? nodeName = node.TryGetIdentifierName();

            if (this.query.HasScope && this.query.ScopeMatchers.Any(x => x.Match(node)))
            {
                this.scope = node;
            }

            if (
                // do not include nodes without the name in results
                nodeName != null
                && (!this.query.HasTarget || this.query.TargetMatchers.Any(x => x.Match(node)))
                && (!this.query.HasScope || (this.scope != null && node.HasParent(this.scope)))
                && (!this.query.HasAccessModifiers
                    || this.query.AccessModifierMatchers.Any(x => x.Match(node)))
                && (!this.query.HasPathMatchers || this.query.PathMatchers.All(x => x.Match(node))))
            {
                this.results.Add(this.CreateResultFromNode(node));
            }

            base.DefaultVisit(node);
        }
    }
}
