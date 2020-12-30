using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxNodeMatchers;
using dngrep.core.Queries.SyntaxNodeMatchers.Boolean;
using dngrep.core.VirtualNodes.VirtualQueries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
                descriptor.AccessModifier == QueryAccessModifier.Any
                    ? Array.Empty<ISyntaxNodeMatcher>()
                    : new[] { new AccessModifierSyntaxNodeMatcher(
                        GetTargetAccessModifiers(descriptor.AccessModifier))
                    },
                GetPathMatchers(
                    descriptor.TargetPathContains,
                    descriptor.TargetPathExcludes,
                    descriptor.EnableRegex));
        }

        public static CombinedSyntaxTreeQuery From(TextSpan nodeSpan)
        {
            var positionMatcher = new SourceTextPositionMatcher(nodeSpan);

            return new CombinedSyntaxTreeQuery(
                new IVirtualNodeQuery[] {
                    MethodBodyVirtualQuery.Instance,
                    NestedBlockVirtualQuery.Instance,
                    AutoPropertyVirtualQuery.Instance,
                    ReadOnlyPropertyVirtualQuery.Instance,
                    TryBodyVirtualQuery.Instance,
                    IfConditionVirtualQuery.Instance,
                    IfBodyVirtualQuery.Instance,
                },
                new SyntaxTreeQuery(
                    new[]
                    {
                        positionMatcher
                    },
                    Array.Empty<ISyntaxNodeMatcher>(),
                    Array.Empty<ISyntaxNodeMatcher>(),
                    Array.Empty<ISyntaxNodeMatcher>()),
                new SyntaxTreeQuery(
                    new[] { positionMatcher },
                    Array.Empty<ISyntaxNodeMatcher>(),
                    Array.Empty<ISyntaxNodeMatcher>(),
                    Array.Empty<ISyntaxNodeMatcher>()
                    ));
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
            QueryTarget.MethodParameter => typeof(ParameterSyntax),
            QueryTarget.InvocationArgument => typeof(ArgumentSyntax),
            _ => throw new NotImplementedException($"The requested target isn't registered. Kind: {target}"),
        };

        private static IReadOnlyCollection<SyntaxKind>
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

        private static IReadOnlyCollection<ISyntaxNodeMatcher> GetPathMatchers(
            IEnumerable<string>? includes,
            IEnumerable<string>? excludes,
            bool regex)
        {
            ISyntaxNodeMatcher CreateMatcher(string pattern)
            {
                ISyntaxNodeMatcher matcher;

                if (regex)
                {
                    matcher = new ContainsPathRegexMatcher(pattern);
                }
                else
                {
                    matcher = new ContainsPathMatcher(pattern);
                }

                return matcher;
            }

            IEnumerable<ISyntaxNodeMatcher> pathContains = includes == null
            || includes.All(x => string.IsNullOrWhiteSpace(x))
            ? Array.Empty<ISyntaxNodeMatcher>()
            : includes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => CreateMatcher(x));

            IEnumerable<ISyntaxNodeMatcher> pathExclude = excludes == null
            || excludes.All(x => string.IsNullOrWhiteSpace(x))
            ? Array.Empty<ISyntaxNodeMatcher>()
            : excludes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select<string, ISyntaxNodeMatcher>(x => new Not(CreateMatcher(x)));

            return pathContains.Concat(pathExclude).ToArray();
        }
    }
}
