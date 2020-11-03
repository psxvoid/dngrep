using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class ContainsNameSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private readonly Type? nodeType;
        private readonly IEnumerable<string>? contains;
        private readonly IEnumerable<string>? exclude;

        public ContainsNameSyntaxNodeMatcher(
            Type? nodeType,
            IEnumerable<string>? contains,
            IEnumerable<string>? exclude)
        {
            this.nodeType = nodeType;
            this.contains = contains;
            this.exclude = exclude;
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();

            if (this.nodeType != null && this.nodeType != nodeType)
            {
                return false;
            }

            string? nodeName = node.TryGetIdentifierName();

            if (nodeName == null)
            {
                return false;
            }

            if (!this.contains.IsNullOrEmpty() && !this.contains.Any(x => nodeName.Contains(x)))
            {
                return false;
            }

            if (!this.exclude.IsNullOrEmpty() && this.exclude.Any(x => nodeName.Contains(x)))
            {
                return false;
            }

            return true;
        }
    }
}
