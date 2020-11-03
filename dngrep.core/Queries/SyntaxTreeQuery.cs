using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQuery
    {
        public IReadOnlyCollection<ISyntaxNodeMatcher> TargetMatchers { get; }

        public IReadOnlyCollection<ISyntaxNodeMatcher> ScopeMatchers { get; }

        public IReadOnlyCollection<SyntaxKind> TargetAccessModifiers { get; }

        public bool HasTarget =>
            this.TargetMatchers != null && this.TargetMatchers.Count > 0;

        public bool HasScope =>
            this.ScopeMatchers != null && this.ScopeMatchers.Count > 0;

        internal SyntaxTreeQuery(
            IReadOnlyCollection<ISyntaxNodeMatcher> targetMatchers,
            IReadOnlyCollection<ISyntaxNodeMatcher> scopeMatchers,
            IReadOnlyCollection<SyntaxKind> targetAccessModifiers
            )
        {
            this.TargetMatchers = targetMatchers;
            this.ScopeMatchers = scopeMatchers;
            this.TargetAccessModifiers = targetAccessModifiers;
        }

    }
}
