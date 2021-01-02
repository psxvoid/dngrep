using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.SyntaxNodeMatchers.Targets
{
    public class MethodBodyParentSyntaxNodeMatcher : ISyntaxNodeMatcher
    {
        private static readonly MethodBodyParentSyntaxNodeMatcher instance =
            new MethodBodyParentSyntaxNodeMatcher();

        private readonly static Type[] MethodBodyParents = new[]
        {
            typeof(MethodDeclarationSyntax),
            typeof(ConstructorDeclarationSyntax),
            typeof(DestructorDeclarationSyntax),
            typeof(LocalFunctionStatementSyntax),
            typeof(ParenthesizedLambdaExpressionSyntax),
            //typeof(PropertyDeclarationSyntax),
            //typeof(AccessorDeclarationSyntax),

            // the expression statement contains nested expressions
            typeof(ExpressionStatementSyntax),
        };

        private MethodBodyParentSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            bool isKnownType = MethodBodyParents.Contains(node.GetType());

            if (!isKnownType)
            {
                return false;
            }

            if (node is MethodDeclarationSyntax method
                && (method.Body != null || method.ExpressionBody != null))
            {
                return true;
            }

            if (node is ConstructorDeclarationSyntax ctor
                && (ctor.Body != null || ctor.ExpressionBody != null))
            {
                return true;
            }

            if (node is DestructorDeclarationSyntax dtor
                && (dtor.Body != null || dtor.ExpressionBody != null))
            {
                return true;
            }

            if (node is ParenthesizedLambdaExpressionSyntax lambda
                && (lambda.Body != null || lambda.ExpressionBody != null))
            {
                return true;
            }

            if (node is LocalFunctionStatementSyntax localFunc
                && (localFunc.Body != null || localFunc.ExpressionBody != null))
            {
                return true;
            }

            if (node is PropertyDeclarationSyntax prop
                && (prop.AccessorList == null && prop.ExpressionBody != null))
            {
                return true;
            }

            if (node is AccessorDeclarationSyntax accessor
                && (accessor.Body != null || accessor.ExpressionBody != null))
            {
                return true;
            }
            
            if (node is BlockSyntax)
            {
                return true;
            }

            if (node is IfStatementSyntax)
            {
                return true;
            }

            return false;
        }

        public static MethodBodyParentSyntaxNodeMatcher Instance => instance;
    }
}
