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

            var method = node as MethodDeclarationSyntax;
            return method != null && method.Body != null;
        }

        public IVirtualSyntaxNode Query(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            var methodDeclaration = node as MethodDeclarationSyntax;

            _ = methodDeclaration ?? throw new ArgumentException(
                "This query can only handle MethodDeclarationSyntax nodes.");

            _ = methodDeclaration.Body ?? throw new InvalidOperationException(
                "This query can only handle MethodDeclarationSyntax nodes with a body.");

            // TODO: How to deal with abstract methods?
            return new MethodBodyDeclarationSyntax(methodDeclaration.Body);
        }
    }
}
