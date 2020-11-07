using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Extensions.SyntaxTreeExtensions
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
                flatNodes.AddRange(node.ChildNodes().GetChildNodesRecursively());
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
        public static IEnumerable<T> GetNodesOfTypeRecursively<T>(this IEnumerable<SyntaxNode> nodes)
            where T : SyntaxNode
        {
            if (nodes == null || !nodes.Any())
            {
                return Enumerable.Empty<T>();
            }

            var flatNodes = new List<T>(nodes.OfType<T>());

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

            string? identifier = syntaxNode.TryGetIdentifierName();

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
                VariableDeclaratorSyntax varDeclarator => varDeclarator.Identifier.ValueText,
                VariableDeclarationSyntax variable => variable?.Variables[0]?.Identifier.ValueText,
                LocalDeclarationStatementSyntax localVariable =>
                    localVariable?.Declaration?.Variables[0]?.Identifier.ValueText,
                EventDeclarationSyntax @event => @event.Identifier.ValueText,
                EnumMemberDeclarationSyntax enumMember => enumMember.Identifier.ValueText,
                NamespaceDeclarationSyntax @namespace => @namespace.Name.ToString(),
                FieldDeclarationSyntax @field =>
                    field?.Declaration?.Variables[0]?.Identifier.ValueText,
                ParameterSyntax param => param.Identifier.ValueText,
                ArgumentSyntax arg => arg.ToString(),
                EventFieldDeclarationSyntax eventField =>
                    eventField?.Declaration?.Variables[0]?.Identifier.ValueText,
                _ => null,
            };
        }

        /// <summary>
        /// Tries to retrieve a file path of a node.
        /// </summary>
        /// <param name="syntaxNode">
        /// The node for which a path should be retrieved.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided node is <see langword="null"/>.
        /// </exception>
        /// <returns>
        /// A file path of the node or <see langword="null"/> when the path doesn't exist.
        /// </returns>
        public static string? TryGetFilePath(this SyntaxNode syntaxNode)
        {
            _ = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));

            return syntaxNode.GetLocation()?.SourceTree?.FilePath;
        }

        /// <summary>
        /// Gets a syntax node full name, including containing
        /// method, class, namespace, etc.
        /// </summary>
        /// <param name="target">
        /// The target node for which the full name should be retrieved.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the target node does not have a name.
        /// For example, <see cref="BlockSyntax"/> does not have a name.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the target node is set to <see cref="null"/>.
        /// </exception>
        /// <returns>
        /// The full name of the syntax node. For example,
        /// for a method it could be MyNamespace.MyClass.MyMethod
        /// instead of just MyMethod.
        /// </returns>
        public static string GetFullName(this SyntaxNode? target)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            return target.TryGetFullName()
                ?? throw new InvalidOperationException(
                    $"The source node does not have a name. Kind: {target.Kind()} Node: {target}");
        }

        /// <summary>
        /// Tries to get a syntax node full name, including containing
        /// method, class, namespace, etc.
        /// </summary>
        /// <param name="target">
        /// The target node for which the full name should be retrieved.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the target node is set to <see langword="null"/>.
        /// </exception>
        /// <returns>
        /// The full name of the syntax node or <see langword="null"/>
        /// if the node does not have a name.
        /// For example, for a method it could be MyNamespace.MyClass.MyMethod
        /// instead of just MyMethod.
        /// </returns>
        public static string? TryGetFullName(this SyntaxNode? target, bool ignoreNamespaces = false)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            var sb = new StringBuilder();
            bool isFirstOccurence = true;

            while (target != null && target.GetType() != typeof(CompilationUnitSyntax))
            {
                string? targetName = target.TryGetIdentifierName();

                if (targetName != null)
                {
                    if (!isFirstOccurence
                        && ignoreNamespaces
                        && target.GetType() == typeof(NamespaceDeclarationSyntax))
                    {
                        target = target.Parent;
                        continue;
                    }

                    if (!isFirstOccurence)
                    {
                        sb.Insert(0, '.');
                    }

                    sb.Insert(0, targetName);
                    isFirstOccurence = false;
                }
                else if (isFirstOccurence)
                {
                    return null;
                }

                target = target.Parent;
            }

            return sb.ToString();
        }
    }
}
