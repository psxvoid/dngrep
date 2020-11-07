using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class ContainsPathMatcher : ISyntaxNodeMatcher
    {
        private readonly string pathSubstring;

        public ContainsPathMatcher(string pathSubstring)
        {
            _ = pathSubstring ?? throw new ArgumentNullException(nameof(pathSubstring));
            _ = !string.IsNullOrWhiteSpace(pathSubstring)
                ? false
                : throw new ArgumentException(
                    "The containing path substring can't be empty.",
                    nameof(pathSubstring));

            this.pathSubstring = pathSubstring;
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return (node.TryGetFilePath() ?? string.Empty).Contains(this.pathSubstring);
        }
    }
}
