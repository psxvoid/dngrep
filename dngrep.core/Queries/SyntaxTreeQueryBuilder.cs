using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxNodeMatchers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries
{
    public static class SyntaxTreeQueryBuilder
    {
        public static SyntaxTreeQuery From(SyntaxTreeQueryDescriptor descriptor)
        {
            _ = descriptor ?? throw new ArgumentNullException(nameof(descriptor));

            Type? target = GetTargetSyntaxNodeType(descriptor.Target);
            Type? scope = GetTargetScopeSyntaxNodeType(descriptor.Scope);
            IReadOnlyCollection<SyntaxKind> modifiers = GetTargetAccessModifiers(descriptor.AccessModifier);


            return new SyntaxTreeQuery(
                descriptor.Target == QueryTarget.Any
                ? Array.Empty<ISyntaxNodeMatcher>()
                : descriptor.EnableRegex
                    ? new[]
                    {
                        (ISyntaxNodeMatcher) new ContainsNameRegexSyntaxNodeMatcher(
                            GetTargetSyntaxNodeType(descriptor.Target),
                            descriptor.TargetNameContains
                                ?.Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => new Regex(x)),
                            descriptor.TargetNameExcludes
                                ?.Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => new Regex(x)))
                    }
                    : new[]
                    {
                        (ISyntaxNodeMatcher) new ContainsNameSyntaxNodeMatcher(
                            GetTargetSyntaxNodeType(descriptor.Target),
                            descriptor.TargetNameContains
                                ?.Where(x => !string.IsNullOrWhiteSpace(x)),
                            descriptor.TargetNameExcludes
                                ?.Where(x => !string.IsNullOrWhiteSpace(x)))
                    },
                descriptor.Scope == QueryTargetScope.None
                ? Array.Empty<ISyntaxNodeMatcher>()
                : descriptor.EnableRegex
                    ? new[]
                    {
                        (ISyntaxNodeMatcher) new ContainsNameRegexSyntaxNodeMatcher(
                            GetTargetScopeSyntaxNodeType(descriptor.Scope),
                            descriptor.TargetScopeContains
                                ?.Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => new Regex(x)),
                            descriptor.TargetScopeExcludes
                                ?.Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => new Regex(x)))
                    }
                    : new[]
                    {
                        (ISyntaxNodeMatcher) new ContainsNameSyntaxNodeMatcher(
                            GetTargetScopeSyntaxNodeType(descriptor.Scope),
                            descriptor.TargetScopeContains
                                ?.Where(x => !string.IsNullOrWhiteSpace(x)),
                            descriptor.TargetNameExcludes
                                ?.Where(x => !string.IsNullOrWhiteSpace(x)))
                    },
                modifiers);
        }

        private static Type? GetTargetSyntaxNodeType(QueryTarget target) => target switch
        {
            QueryTarget.Any => (Type?)null,
            QueryTarget.Class => typeof(ClassDeclarationSyntax),
            QueryTarget.Enum => typeof(EnumDeclarationSyntax),
            QueryTarget.Field => typeof(FieldDeclarationSyntax),
            QueryTarget.Interface => typeof(InterfaceDeclarationSyntax),
            QueryTarget.Struct => typeof(StructDeclarationSyntax),
            QueryTarget.Method => typeof(MethodDeclarationSyntax),
            QueryTarget.Namespace => typeof(NamespaceDeclarationSyntax),
            QueryTarget.Property => typeof(PropertyDeclarationSyntax),
            QueryTarget.LocalVariable => typeof(LocalDeclarationStatementSyntax),
            QueryTarget.MethodArgument => typeof(ArgumentSyntax),
            _ => throw new NotImplementedException($"The requested target isn't registered. Kind: {target}"),
        };

        public static IReadOnlyCollection<SyntaxKind>
            GetTargetAccessModifiers(QueryAccessModifier accessModifier) => accessModifier switch
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

        private static Type? GetTargetScopeSyntaxNodeType(QueryTargetScope scope) => scope switch
        {
            QueryTargetScope.None => (Type?)null,
            QueryTargetScope.Class => typeof(ClassDeclarationSyntax),
            QueryTargetScope.Struct => typeof(StructDeclarationSyntax),
            QueryTargetScope.Namespace => typeof(NamespaceDeclarationSyntax),
            QueryTargetScope.Interface => typeof(InterfaceDeclarationSyntax),
            _ => throw new NotImplementedException($"The requested scope isn't registered. Kind: {scope}"),
        };
    }
}
