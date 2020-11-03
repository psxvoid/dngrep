using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries.SyntaxNodeMatchers
{
    public class AccessModifierSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private readonly IReadOnlyCollection<SyntaxKind> modifiers;

        public AccessModifierSyntaxNodeMatcher(IReadOnlyCollection<SyntaxKind> modifiers)
        {
            this.modifiers = modifiers;
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return this.modifiers.Count ==
                node.ChildTokens()
                .Select(x => x.Kind())
                .Intersect(this.modifiers).Count();
        }
    }
}
