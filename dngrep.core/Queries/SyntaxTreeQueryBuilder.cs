using dngrep.core.Queries.Specifiers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace dngrep.core.Queries
{
    public static class SyntaxTreeQueryBuilder
    {
        public static SyntaxTreeQuery From(SyntaxTreeQueryDescriptor queryDescriptor)
        {
            _ = queryDescriptor ?? throw new ArgumentNullException(nameof(queryDescriptor));

            Type target = GetTargetSyntaxNodeType(queryDescriptor.Target);
            Type scope = GetTargetScopeSyntaxNodeType(queryDescriptor.Scope);

            return new SyntaxTreeQuery(
                target,
                scope,
                queryDescriptor.QueryTargetName,
                queryDescriptor.QueryTargetScopeName);
        }

        private static Type GetTargetSyntaxNodeType(QueryTarget target)
        {
            return target switch
            {
                QueryTarget.Class => typeof(ClassDeclarationSyntax),
                QueryTarget.Struct => typeof(StructDeclarationSyntax),
                QueryTarget.Method => typeof(MethodDeclarationSyntax),
                _ => throw new NotImplementedException("The requested target isn't registered."),
            };
        }

        private static Type GetTargetScopeSyntaxNodeType(QueryTargetScope scope)
        {
            return scope switch
            {
                QueryTargetScope.Class => typeof(ClassDeclarationSyntax),
                QueryTargetScope.Struct => typeof(StructDeclarationSyntax),
                _ => throw new NotImplementedException("The requested scope isn't registered."),
            };
        }
    }
}
