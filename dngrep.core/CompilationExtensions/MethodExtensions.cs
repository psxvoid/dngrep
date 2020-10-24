using dngrep.core.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dngrep.core.CompilationExtensions
{
    public static class MethodExtensions
    {
        public static IReadOnlyCollection<string> GetMethodNames(this CSharpCompilation compilation)
        {
            _ = compilation ?? throw new ArgumentNullException(nameof(compilation));

            var names = new List<string>();
            foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
            {
                IEnumerable<SyntaxNode>? childNodes = syntaxTree.GetRoot().ChildNodes();
                IEnumerable<SyntaxNode> syntaxNodes = childNodes.GetNodesOfTypeRecursively<MethodDeclarationSyntax>();

                // field declarations may contain multiple variable declarations
                // we have to catch them all
                IEnumerable<FieldDeclarationSyntax>? fieldDeclarations = syntaxNodes.OfType<FieldDeclarationSyntax>();
                IEnumerable<VariableDeclaratorSyntax>? fields = fieldDeclarations.SelectMany(x => x.Declaration.Variables);

                Type fieldDeclarationType = typeof(FieldDeclarationSyntax);
                IEnumerable<SyntaxNode>? namedNodes = syntaxNodes
                    .Where(x => x.GetType() != fieldDeclarationType)
                    .Concat(fields);

                names.AddRange(namedNodes.Select(x => x.GetIdentifierName()));
            }

            return names;
        }
    }
}
