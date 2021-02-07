using System;
using System.Collections.Generic;

namespace dngrep.core.Queries
{
    public class BasicSyntaxTreeQuery
    {
        public BasicSyntaxTreeQuery(
            IReadOnlyCollection<ISyntaxNodeMatcher> matchers,
            IReadOnlyCollection<IVirtualSyntaxNodeMatcher> virtualMatchers,
            IReadOnlyCollection<IVirtualNodeQuery> virtualQueries)
        {
            this.Matchers = matchers ?? Array.Empty<ISyntaxNodeMatcher>();
            this.VirtualMatchers = virtualMatchers ?? Array.Empty<IVirtualSyntaxNodeMatcher>();
            this.VirtualQueries = virtualQueries ?? Array.Empty<IVirtualNodeQuery>();
        }

        public IReadOnlyCollection<ISyntaxNodeMatcher> Matchers { get; }
        public IReadOnlyCollection<IVirtualSyntaxNodeMatcher> VirtualMatchers { get; }
        public IReadOnlyCollection<IVirtualNodeQuery> VirtualQueries { get; }
    }
}
