using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    class ContainsNameRegexSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private readonly Type? nodeType;
        private readonly IEnumerable<Regex>? contains;
        private readonly IEnumerable<Regex>? exclude;

        public ContainsNameRegexSyntaxNodeMatcher(Type? nodeType, IEnumerable<Regex>? contains, IEnumerable<Regex>? exclude)
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

            if (this.contains != null && !this.contains.Any(x => x.IsMatch(nodeName)))
            {
                return false;
            }

            if(this.exclude != null && this.exclude.Any(x => x.IsMatch(nodeName)))
            {
                return false;
            }

            return true;
        }
    }
}
