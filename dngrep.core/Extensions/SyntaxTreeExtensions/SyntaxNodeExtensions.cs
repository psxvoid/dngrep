using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
        /// Flattens and entire child node hierarchy into a single enumerable.
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

            foreach (SyntaxNode node in nodes)
            {
                flatNodes.AddRange(node.ChildNodes().GetChildNodesRecursively());
            }

            return flatNodes;
        }

        /// <summary>
        /// Gets the first child node of a specified type in the syntax-node child chain.
        /// Throws an exception when the node of the specified type cannot be found.
        /// </summary>
        /// <typeparam name="T">The type of the child node to find.</typeparam>
        /// <param name="node">The node, containing the target node of the specified type.</param>
        /// <returns>The child node the provided node that has the specified type.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided node doesn't have any child or no any target nodes are found.
        /// </exception>
        public static T GetFirstChildOfTypeRecursively<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            IEnumerable<SyntaxNode>? child = node.ChildNodes();

            if (child == null || !child.Any())
            {
                throw new InvalidOperationException("The node does not contain any child nodes");
            }

            IEnumerable<T> nodes = child.GetNodesOfTypeRecursively<T>();

            if (nodes == null || !nodes.Any())
            {
                throw new InvalidOperationException(
                    "Unable to find any nodes of the target type.");
            }

            return nodes.First();
        }

        /// <summary>
        /// Gets child nodes of a specified type in the syntax-node child chain.
        /// </summary>
        /// <typeparam name="T">The type of the child node to find.</typeparam>
        /// <param name="node">The node, containing the target nodes of the specified type.</param>
        /// <returns>
        /// The child nodes of the provided node that have the specified type.
        /// Returns an empty enumerable when no any nodes are found.
        /// </returns>
        public static IEnumerable<T> GetNodesOfTypeRecursively<T>(this IEnumerable<SyntaxNode> nodes)
            where T : SyntaxNode
        {
            if (nodes == null || !nodes.Any())
            {
                return Enumerable.Empty<T>();
            }

            var flatNodes = new List<T>(nodes.OfType<T>());

            foreach (SyntaxNode node in nodes)
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
        /// Gets the bounds of the <see cref="SyntaxNode"/> in the source text.
        /// </summary>
        /// <param name="node">The node for which source bounds should be retrieved.</param>
        /// <returns>The bounds of the node in the source text.</returns>
        public static (int lineStart, int lineEnd, int charStart, int charEnd)
            GetSourceTextBounds(this SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            FileLinePositionSpan lineSpan = node.GetLocation().GetLineSpan();
            LinePosition startPosition = lineSpan.StartLinePosition;
            LinePosition endPosition = lineSpan.EndLinePosition;

            return (
                startPosition.Line,
                endPosition.Line,
                startPosition.Character,
                endPosition.Character);
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
            bool isFirstOccurrence = true;

            while (target != null && target.GetType() != typeof(CompilationUnitSyntax))
            {
                string? targetName = target.TryGetIdentifierName();

                if (targetName != null)
                {
                    if (!isFirstOccurrence
                        && ignoreNamespaces
                        && target.GetType() == typeof(NamespaceDeclarationSyntax))
                    {
                        target = target.Parent;
                        continue;
                    }

                    if (!isFirstOccurrence)
                    {
                        sb.Insert(0, '.');
                    }

                    sb.Insert(0, targetName);
                    isFirstOccurrence = false;
                }
                else if (isFirstOccurrence)
                {
                    return null;
                }

                target = target.Parent;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the first parent <see cref="SyntaxNode"/> of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the parent syntax node to get.</typeparam>
        /// <param name="target">
        /// The node for which a parent of a particular type should be found.
        /// </param>
        /// <returns>The first parent of the specified type or <see langword="null"/>.</returns>
        public static T? GetFirstParentOfType<T>(this SyntaxNode target)
            where T : SyntaxNode
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            SyntaxNode? parent = target.Parent;

            while (parent != null)
            {
                if (parent.GetType() == typeof(T))
                {
                    return parent as T;
                }

                parent = parent.Parent;
            }

            return null;
        }

        /// <summary>
        /// Gets the method body from the provided node. The same as
        /// <see cref="TryGetBody(SyntaxNode)"/> but will throw an
        /// exception when the method body isn't found in the provided
        /// node.
        /// </summary>
        /// <param name="nodeWithBody">
        /// The node that may contain a method body. This method will
        /// to get the method body from this node. It also can be assumed
        /// that the provided node should be a method body parent.
        /// </param>
        /// <returns>
        /// The body of the provided node. Can be <see cref="BlockSyntax"/>,
        /// <see cref="ArrowExpressionClauseSyntax"/>, or any child of
        /// <see cref="ExpressionSyntax"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <see langword="null"/> is passed as the node argument.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the method is unable to find a method body for the
        /// specified node argument.
        /// </exception>
        public static SyntaxNode GetBody(this SyntaxNode nodeWithBody)
        {
            _ = nodeWithBody ?? throw new ArgumentNullException(nameof(nodeWithBody));

            return TryGetBody(nodeWithBody) ?? throw new InvalidOperationException(
                "Unable to get the body for the node.");
        }

        /// <summary>
        /// Tries to get a method body from the provided node. The same
        /// as <see cref="GetBody(SyntaxNode)"/> but won't thrown an
        /// exception when the body isn't found.
        /// </summary>
        /// <param name="nodeWithBody">
        /// The node that may contain a method body. This method will try
        /// to get the method body from this node. It also can be assumed
        /// that the provided node can be a potential method body parent.
        /// </param>
        /// <returns>
        /// The body of the provided node. Can be <see cref="BlockSyntax"/>,
        /// <see cref="ArrowExpressionClauseSyntax"/>, or any child of
        /// <see cref="ExpressionSyntax"/>. Will be <see langword="null"/>
        /// when the body isn't found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <see langword="null"/> is passed as the node argument.
        /// </exception>
        public static SyntaxNode? TryGetBody(this SyntaxNode nodeWithBody)
        {
            _ = nodeWithBody ?? throw new ArgumentNullException(nameof(nodeWithBody));

            BlockSyntax? blockBody = null;
            ArrowExpressionClauseSyntax? arrowExpressionBody = null;
            ExpressionSyntax? expressionSyntax = null;

            if (nodeWithBody is MethodDeclarationSyntax method)
            {
                blockBody = method.Body;
                arrowExpressionBody = method.ExpressionBody;
            }
            else if (nodeWithBody is ConstructorDeclarationSyntax ctor)
            {
                blockBody = ctor.Body;
                arrowExpressionBody = ctor.ExpressionBody;
            }
            else if (nodeWithBody is AccessorDeclarationSyntax accessor)
            {
                blockBody = accessor.Body;
                arrowExpressionBody = accessor.ExpressionBody;
            }
            else if (nodeWithBody is AnonymousFunctionExpressionSyntax anonymouseFunc)
            {
                blockBody = anonymouseFunc.Block;
                expressionSyntax = anonymouseFunc.ExpressionBody;
            }
            else if (nodeWithBody is PropertyDeclarationSyntax prop)
            {
                arrowExpressionBody = prop.ExpressionBody;
            }

            return (SyntaxNode?)blockBody
                ?? (SyntaxNode?)arrowExpressionBody
                ?? (SyntaxNode?)expressionSyntax;
        }

        /// <summary>
        /// Verifies whether a node is a container for another syntax constructs
        /// like statements. For example, it will return true for a BlockSyntax
        /// that may contain another code inside.
        /// </summary>
        /// <param name="node">
        /// The node that should be analyzed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the node is a container for another syntax
        /// constructs, else <see langword="false"/>.
        /// </returns>
        public static bool IsContainer(this SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return node is BlockSyntax
                || node is ArrowExpressionClauseSyntax
                || node is ExpressionSyntax
                && !(node is TypeSyntax);
        }
    }
}
