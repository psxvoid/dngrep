using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.core.Queries.Specifiers
{
    public enum QueryTarget
    {
        Any,
        Namespace,
        Enum,
        Interface,
        Class,
        Struct,
        Field,
        Property,
        Method,
        InvocationArgument,
        LocalVariable,
        MethodParameter,
    }

    public static class KnownQueryTargets
    {
        public static readonly IReadOnlyCollection<QueryTarget> UniqueTargets =
            Enum.GetValues(typeof(QueryTarget))
                .Cast<QueryTarget>()
                .Where(x => x != QueryTarget.Any)
                .ToArray();

        public static readonly IReadOnlyCollection<Type> EqualSyntaxNodeTypes =
            UniqueTargets
            .Select(x => GetAssociatedSyntaxNodeType(x))
            .ToArray();

        private static Type GetAssociatedSyntaxNodeType(QueryTarget target) => target switch
        {
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
            _ => throw new NotImplementedException($"The requested target isn't supported. Kind: {target}"),
        };
    }
}
