using dngrep.core.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

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
            this.query = query;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();

            if (this.query.ScopeType != null && nodeType == this.query.ScopeType)
            {
                if (string.IsNullOrWhiteSpace(this.query.TargetScopeName)
                 || node.GetIdentifierName().Contains(this.query.TargetScopeName))
                {
                    this.scope = node;
                }
            }

            if (this.query.TargetType == nodeType)
            {
                if (
                    // match target name
                    (string.IsNullOrWhiteSpace(this.query.TargetName) || node.GetIdentifierName().Contains(this.query.TargetName))
                    // match scope type
                    && (this.query.ScopeType == null || this.scope != null && node.HasParent(this.scope))
                    // match access modifiers
                    && (this.query.TargetAccessModifiers == null || this.query.TargetAccessModifiers.Count == 0
                        || node.ChildTokens().Select(x => x.Kind()).Intersect(this.query.TargetAccessModifiers).Count()
                            == this.query.TargetAccessModifiers.Count))
                {
                    this.results.Add(node);
                }
            }

            base.DefaultVisit(node);
        }
    }
}
