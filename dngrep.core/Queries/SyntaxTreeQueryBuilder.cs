﻿using dngrep.core.Queries.Specifiers;
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
            Type? scope = GetTargetScopeSyntaxNodeType(queryDescriptor.Scope);
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
                QueryTarget.Enum => typeof(EnumDeclarationSyntax),
                QueryTarget.Field => typeof(FieldDeclarationSyntax),
                QueryTarget.Interface => typeof(InterfaceDeclarationSyntax),
                QueryTarget.Struct => typeof(StructDeclarationSyntax),
                QueryTarget.Method => typeof(MethodDeclarationSyntax),
                QueryTarget.Namespace => typeof(NamespaceDeclarationSyntax),
                QueryTarget.Property => typeof(PropertyDeclarationSyntax),
                QueryTarget.Variable => typeof(VariableDeclaratorSyntax),
                _ => throw new NotImplementedException($"The requested target isn't registered. Kind: {target}"),
            };
        }

        public static IReadOnlyCollection<SyntaxKind> GetTargetAccessModifiers(QueryAccessModifier accessModifier)
        {
            return accessModifier switch
            {
                QueryAccessModifier.Any => Array.Empty<SyntaxKind>(),
                QueryAccessModifier.Public => new[] { SyntaxKind.PublicKeyword },
                QueryAccessModifier.Private => new[] { SyntaxKind.PrivateKeyword },
                QueryAccessModifier.Internal => new[] { SyntaxKind.InternalKeyword },
                QueryAccessModifier.Protected => new[] { SyntaxKind.ProtectedKeyword },
                QueryAccessModifier.ProtectedInternal =>
                    new[] { SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword },
                QueryAccessModifier.PrivateProtected =>
                    new[] { SyntaxKind.PrivateKeyword, SyntaxKind.ProtectedKeyword },
                _ => throw new NotImplementedException($"The requested access modifier isn't registered. Kind: {accessModifier}"),
            };
        }

        private static Type? GetTargetScopeSyntaxNodeType(QueryTargetScope scope)
        {
            return scope switch
            {
                QueryTargetScope.None => (Type?)null,
                QueryTargetScope.Class => typeof(ClassDeclarationSyntax),
                QueryTargetScope.Struct => typeof(StructDeclarationSyntax),
                _ => throw new NotImplementedException($"The requested scope isn't registered. Kind: {scope}"),
            };
        }
    }
}
