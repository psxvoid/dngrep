using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dngrep.core.SyntaxTreeExtensions
{
    public static class SyntaxNodeExtensions
    {
        /// <summary>
        /// Flatterns and entire child node hierarchy into a single enumerable.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static IEnumerable<SyntaxNode> GetChildNodesRecursively(this IEnumerable<SyntaxNode> nodes)
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

        /// <summary>
        /// The difference between this method and <see cref="GetChildNodesRecursively(IEnumerable{SyntaxNode})"/>
        /// is that this method only gets nodes of a specified type and potentially can use less memory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static IEnumerable<SyntaxNode> GetNodesOfTypeRecursively<T>(this IEnumerable<SyntaxNode> nodes)
            where T : SyntaxNode
        {
            if (nodes == null || !nodes.Any())
            {
                return Enumerable.Empty<SyntaxNode>();
            }

            var flatNodes = new List<SyntaxNode>(nodes.OfType<T>());

            foreach (var node in nodes)
            {
                flatNodes.AddRange(node.ChildNodes().GetNodesOfTypeRecursively<T>());
            }

            return flatNodes;
        }

        /// <summary>
        /// Gets  an identifier name from a supported <see cref="SyntaxNode"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a specified <see cref="SyntaxNode"/> does not have an associated name.
        /// </exception>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        public static string GetIdentifierName(this SyntaxNode syntaxNode)
        {
            _ = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));

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
