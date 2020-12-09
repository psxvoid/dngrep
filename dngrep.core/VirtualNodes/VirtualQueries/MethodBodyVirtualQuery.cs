using System;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.VirtualNodes.VirtualQueries
{
    public class MethodBodyVirtualQuery : IVirtualNodeQuery
    {
        public bool HasOverride => false;

        public bool CanQuery(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return node is MethodDeclarationSyntax method
                && (method.Body != null || method.ExpressionBody != null);
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            var methodDeclaration = node as MethodDeclarationSyntax;

            _ = methodDeclaration ?? throw new ArgumentException(
                "This query can only handle MethodDeclarationSyntax nodes.");

            SyntaxNode? body = methodDeclaration.Body != null
                ? (SyntaxNode?) methodDeclaration.Body
                : (SyntaxNode?) methodDeclaration.ExpressionBody;

            _ = body ?? throw new InvalidOperationException(
                "This query can only handle MethodDeclarationSyntax nodes with a body.");

            return new MethodBodyDeclarationSyntax(body);
        }
    }
}
