using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dngrep.core.CompilationExtensions
{
    public static class CommonExtensions
    {
        private static HashSet<Type> CommonSymbols = new HashSet<Type>()
        {
            typeof(InterfaceDeclarationSyntax),
            typeof(EnumDeclarationSyntax),
            typeof(ClassDeclarationSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(PropertyDeclarationSyntax),
            typeof(MethodDeclarationSyntax),
            typeof(VariableDeclaratorSyntax),
            typeof(EventDeclarationSyntax),
            typeof(EnumMemberDeclarationSyntax),
            typeof(StructDeclarationSyntax),
        };

        /// <summary>
        /// Reads all parts of the code which can be useful for grepping.
        /// Includes:
        /// <list type="bullet">
        /// <item>interfaces</item>
        /// <item>enums</item>
        /// <item>enum members</item>
        /// <item>structs</item>
        /// <item>classes</item>
        /// <item>fields</item>
        /// <item>properties</item>
        /// <item>events</item>
        /// <item>methods</item>
        /// <item>method variables</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<string> GetCommonSymbolNames(this CSharpCompilation compilation)
        {
            _ = compilation ?? throw new ArgumentNullException(nameof(compilation));

            List<string> names = new List<string>();
            foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
            {
                IEnumerable<SyntaxNode>? childNodes = syntaxTree.GetRoot().ChildNodes();
                IEnumerable<SyntaxNode> syntaxNodes = GetChildNodesRecursively(childNodes)
                    .Where(x => CommonSymbols.Contains(x.GetType()));

                // field declarations may contain multiple variable declarations
                // we have to catch them all
                IEnumerable<FieldDeclarationSyntax>? fieldDeclarations = syntaxNodes.OfType<FieldDeclarationSyntax>();
                IEnumerable<VariableDeclaratorSyntax>? fields = fieldDeclarations.SelectMany(x => x.Declaration.Variables);

                Type fieldDeclarationType = typeof(FieldDeclarationSyntax);
                IEnumerable<SyntaxNode>? namedNodes = syntaxNodes
                    .Where(x => x.GetType() != fieldDeclarationType)
                    .Concat(fields);

                names.AddRange(namedNodes.Select(x => GetIdentifierName(x)));
            }

            return names;
        }

        private static IEnumerable<SyntaxNode> GetChildNodesRecursively(IEnumerable<SyntaxNode> nodes)
        {
            if (nodes == null || !nodes.Any())
            {
                return Enumerable.Empty<SyntaxNode>();
            }

            var flatNodes = new List<SyntaxNode>(nodes);

            foreach (var node in nodes)
            {
                flatNodes.AddRange(GetChildNodesRecursively(node.ChildNodes()));
            }

            return flatNodes;
        }

        private static string GetIdentifierName(SyntaxNode syntaxNode)
        {
            if (syntaxNode is BaseTypeDeclarationSyntax baseType)
            {
                return baseType.Identifier.ValueText;
            }
            else if (syntaxNode is MethodDeclarationSyntax method)
            {
                return method.Identifier.ValueText;
            }
            else if (syntaxNode is PropertyDeclarationSyntax property)
            {
                return property.Identifier.ValueText;
            }
            else if (syntaxNode is VariableDeclaratorSyntax variable)
            {
                return variable.Identifier.ValueText;
            }
            else if (syntaxNode is EventDeclarationSyntax @event)
            {
                return @event.Identifier.ValueText;
            }
            else if (syntaxNode is EnumMemberDeclarationSyntax enumMember)
            {
                return enumMember.Identifier.ValueText;
            }

            throw new InvalidOperationException($"Unable to get identifier node for {syntaxNode.GetType()}");
        }
    }
}
