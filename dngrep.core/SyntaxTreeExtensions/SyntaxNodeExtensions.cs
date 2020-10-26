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
        /// Verifies whether the node contains another node in a parent hierarchy.
        /// </summary>
        /// <param name="target">
        /// The node for which the parent hierarchy should be scanned for a matching parent.
        /// </param>
        /// <param name="parent">
        /// The node that potentially can be in the parent hierarchy of the target node.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the target node contains
        /// another node in it's parent hierarchy, else <see langword="false"/>.
        /// </returns>
        public static bool HasParent(this SyntaxNode target, SyntaxNode parent)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = parent ?? throw new ArgumentNullException(nameof(parent));

            while (target.Parent != null)
            {
                if (ReferenceEquals(target.Parent, parent))
                {
                    return true;
                }

                target = target.Parent;
            }

            return false;
        }

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
        /// <returns>Returns the identifier name.</returns>
        public static string GetIdentifierName(this SyntaxNode syntaxNode)
        {
            _ = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));

            string? identifier = TryGetIdentifierName(syntaxNode);

            return identifier ?? throw new InvalidOperationException(
                $"Unable to get identifier node for {syntaxNode.GetType()}");
        }

        /// <summary>
        /// Gets  an identifier name from a supported <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <returns>Returns the identifier name or <see langword="null"/>.</returns>
        public static string? TryGetIdentifierName(this SyntaxNode syntaxNode)
        {
            _ = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));

            return syntaxNode switch
            {
                BaseTypeDeclarationSyntax baseType => baseType.Identifier.ValueText,
                MethodDeclarationSyntax method => method.Identifier.ValueText,
                PropertyDeclarationSyntax property => property.Identifier.ValueText,
                VariableDeclaratorSyntax variable => variable.Identifier.ValueText,
                EventDeclarationSyntax @event => @event.Identifier.ValueText,
                EnumMemberDeclarationSyntax enumMember => enumMember.Identifier.ValueText,
                NamespaceDeclarationSyntax @namespace => @namespace.Name.ToString(),
                _ => null,
            };
        }
    }
}
