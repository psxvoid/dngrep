using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Extensions.CompilationExtensions
{
    public static class ClassExtensions
    {
        public static IReadOnlyCollection<string> GetClassNames(this CSharpCompilation compilation)
        {
            _ = compilation ?? throw new ArgumentNullException(nameof(compilation));

            var names = new List<string>();
            foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
            {
                IEnumerable<ClassDeclarationSyntax> classDeclarations = syntaxTree
                    .GetRoot()
                    .ChildNodes()
                    .OfType<ClassDeclarationSyntax>();

                names.AddRange(classDeclarations.Select(x => x.Identifier.ValueText));
            }

            return names;
        }
    }
}
