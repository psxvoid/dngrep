using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Extensions.SyntaxTreeExtensions
{
    public static class SyntaxTreeExtensions
    {
        public static IEnumerable<SyntaxNode> NodesOfType<T>(this SyntaxTree tree)
            where T : SyntaxNode
        {
            _ = tree ?? throw new ArgumentNullException(nameof(tree));

            return tree
                .GetRoot()
                .ChildNodes()
                .GetNodesOfTypeRecursively<T>();
        }
    }
}
