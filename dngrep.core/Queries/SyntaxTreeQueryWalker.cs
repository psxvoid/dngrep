using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQueryWalker : CSharpSyntaxWalker
    {
        private readonly List<SyntaxNode> results = new List<SyntaxNode>();
        private readonly SyntaxTreeQuery query;

        private readonly IEnumerable<Regex>? includes;
        private readonly IEnumerable<Regex>? excludes;

        private SyntaxNode? scope;

        public IReadOnlyCollection<SyntaxNode> Results => this.results;

        public SyntaxTreeQueryWalker(SyntaxTreeQuery query)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            this.query = query;
            if (query.EnableRegex)
            {
                bool hasIncludes = query.TargetNameContains != null
                    && query.TargetNameContains.Any(x => !string.IsNullOrEmpty(x));

                bool hasExcludes = query.TargetNameExcludes != null
                    && query.TargetNameExcludes.Any(x => !string.IsNullOrEmpty(x));

                this.includes = hasIncludes
                    ? query.TargetNameContains
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => new Regex(x))
                    : null;

                this.excludes = hasExcludes
                    ? query.TargetNameExcludes
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => new Regex(x))
                    : null;
            }
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();
            string? nodeName = node.TryGetIdentifierName();

            if (this.query.ScopeType != null && nodeType == this.query.ScopeType)
            {
                if (
                    this.query.TargetScopeContains == null
                    || !this.query.TargetScopeContains.Any()
                    || (nodeName != null && this.query.TargetScopeContains.Any(x => nodeName.Contains(x))))
                {
                    this.scope = node;
                }
            }

            if (this.query.TargetType == nodeType || this.query.TargetType == null)
            {
                if (
                    // do not include nodes without the name in results
                    nodeName != null
                    // match target name, OR boolean query
                    && (this.query.TargetNameContains == null
                        || !this.query.TargetNameContains.Any()
                        || (this.includes == null
                            ? this.query.TargetNameContains.Any(
                                x => nodeName.Contains(x))
                            : this.includes.Any(x => x.IsMatch(nodeName))
                            ))
                    // exclude target name, OR boolean query
                    && (this.query.TargetNameExcludes == null
                        || !this.query.TargetNameExcludes.Any()
                        || (this.excludes == null
                            ? !this.query.TargetNameExcludes.Any(
                                x => nodeName.Contains(x))
                            : !this.excludes.Any(x => x.IsMatch(nodeName))
                            ))
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
