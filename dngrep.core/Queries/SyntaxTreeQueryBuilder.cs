using dngrep.core.Queries.Specifiers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace dngrep.core.Queries
{
    public static class SyntaxTreeQueryBuilder
    {
        public static SyntaxTreeQuery From(SyntaxTreeQueryDescriptor queryDescriptor)
        {
            _ = queryDescriptor ?? throw new ArgumentNullException(nameof(queryDescriptor));

            Type target = GetTargetSyntaxNodeType(queryDescriptor.Target);
            Type scope = GetTargetScopeSyntaxNodeType(queryDescriptor.Scope);
            IReadOnlyCollection<SyntaxKind> modifiers = GetTargetAccessModifiers(queryDescriptor.AccessModifier);

            return new SyntaxTreeQuery(
                target,
                modifiers,
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

        public static IReadOnlyCollection<SyntaxKind> GetTargetAccessModifiers(QueryAccessModifier accessModifier)
        {
            var modifiers = new List<SyntaxKind>();

            SyntaxKind modifier = accessModifier switch
            {
                QueryAccessModifier.Any => SyntaxKind.None,
                QueryAccessModifier.Public => SyntaxKind.PublicKeyword,
                QueryAccessModifier.Private => SyntaxKind.PrivateKeyword,
                _ => throw new NotImplementedException("The requested access modifier isn't registered."),
            };

            if (modifier != SyntaxKind.None) modifiers.Add(modifier);

            return modifiers;
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
