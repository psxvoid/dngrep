using System;
using System.Collections.Generic;
using System.Linq;

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

            if (virtualQueries.Any(x => !x.HasOverride))
            {
                throw new ArgumentException(
                    $"The {nameof(BasicSyntaxTreeQuery)} only supports overridable queries.",
                    nameof(virtualQueries));
            }
        }

        public IReadOnlyCollection<ISyntaxNodeMatcher> Matchers { get; }
        public IReadOnlyCollection<IVirtualSyntaxNodeMatcher> VirtualMatchers { get; }
        public IReadOnlyCollection<IVirtualNodeQuery> VirtualQueries { get; }
    }
}
