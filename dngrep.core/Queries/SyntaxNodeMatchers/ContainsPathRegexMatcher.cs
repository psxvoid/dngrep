using System;
using System.Text.RegularExpressions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class ContainsPathRegexMatcher : ISyntaxNodeMatcher
    {
        private readonly Regex pathRegex;

        public ContainsPathRegexMatcher(string pathPattern)
        {
            _ = pathPattern ?? throw new ArgumentNullException(nameof(pathPattern));
            _ = !string.IsNullOrWhiteSpace(pathPattern)
                ? false
                : throw new ArgumentException(
                    "The containing path regex substring can't be empty.",
                    nameof(pathPattern));

            this.pathRegex = new Regex(pathPattern);
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return this.pathRegex.IsMatch(node.TryGetFilePath() ?? string.Empty);
        }
    }
}
