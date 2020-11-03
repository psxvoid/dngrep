using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQueryWalker : CSharpSyntaxWalker
    {
        private readonly List<SyntaxNode> results = new List<SyntaxNode>();
        private readonly SyntaxTreeQuery query;

        private SyntaxNode? scope;

        public IReadOnlyCollection<SyntaxNode> Results => this.results;

        public SyntaxTreeQueryWalker(SyntaxTreeQuery query)
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
                // match target type and name
                && (!this.query.HasTarget || this.query.TargetMatchers.Any(x => x.Match(node)))
                // match scope type
                && (!this.query.HasScope || (this.scope != null && node.HasParent(this.scope)))
                // match access modifiers
                && (this.query.TargetAccessModifiers == null || this.query.TargetAccessModifiers.Count == 0
                    || node.ChildTokens().Select(x => x.Kind()).Intersect(this.query.TargetAccessModifiers).Count()
                        == this.query.TargetAccessModifiers.Count))
            {
                this.results.Add(node);
            }

            base.DefaultVisit(node);
        }
    }
}
