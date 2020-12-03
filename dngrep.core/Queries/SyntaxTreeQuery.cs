using System.Collections.Generic;
using dngrep.core.Extensions.EnumerableExtensions;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQuery
    {
        public IReadOnlyCollection<ISyntaxNodeMatcher> TargetMatchers { get; }

        public IReadOnlyCollection<ISyntaxNodeMatcher> ScopeMatchers { get; }

        public IReadOnlyCollection<ISyntaxNodeMatcher> AccessModifierMatchers { get; }

        public IReadOnlyCollection<ISyntaxNodeMatcher> PathMatchers { get; }

        public bool HasTarget =>
            !this.TargetMatchers.IsNullOrEmpty();

        public bool HasScope =>
            !this.ScopeMatchers.IsNullOrEmpty();

        public bool HasAccessModifiers =>
            !this.AccessModifierMatchers.IsNullOrEmpty();

        public bool HasPathMatchers =>
            !this.PathMatchers.IsNullOrEmpty();

        internal SyntaxTreeQuery(
            IReadOnlyCollection<ISyntaxNodeMatcher> targetMatchers,
            IReadOnlyCollection<ISyntaxNodeMatcher> scopeMatchers,
            IReadOnlyCollection<ISyntaxNodeMatcher> accessModifierMatchers,
            IReadOnlyCollection<ISyntaxNodeMatcher> pathMatchers
            )
        {
            this.TargetMatchers = targetMatchers;
            this.ScopeMatchers = scopeMatchers;
            this.AccessModifierMatchers = accessModifierMatchers;
            this.PathMatchers = pathMatchers;
        }
    }

    public class CombinedSyntaxTreeQuery : SyntaxTreeQuery
    {
        public IReadOnlyCollection<IVirtualNodeQuery> VirtualNodeQueries { get; }

        public SyntaxTreeQuery VirtualNodeSubQueries { get; }

        internal CombinedSyntaxTreeQuery(
            IReadOnlyCollection<IVirtualNodeQuery> virtualNodeQueries,
            SyntaxTreeQuery nodeQuery,
            SyntaxTreeQuery virtualNodeSubQueries)
            : base(
                nodeQuery.TargetMatchers,
                nodeQuery.ScopeMatchers,
                nodeQuery.AccessModifierMatchers,
                nodeQuery.TargetMatchers)
        {
            this.VirtualNodeQueries = virtualNodeQueries;
            this.VirtualNodeSubQueries = virtualNodeSubQueries;
        }
    }
}
