using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQuery
    {
        public Type TargetType { get; }
        public string? TargetName { get; }
        public IReadOnlyCollection<SyntaxKind>? TargetAccessModifiers { get; }

        public Type? ScopeType { get; }
        public string? TargetScopeName { get; }

        internal SyntaxTreeQuery(
            Type targetType,
            IReadOnlyCollection<SyntaxKind>? targetAccessModifiers,
            Type? scopeType,
            string? targetName,
            string? targetScopeName)
        {
            this.TargetType = targetType;
            this.TargetName = targetName;
            this.TargetAccessModifiers = targetAccessModifiers;
            this.ScopeType = scopeType;
            this.TargetScopeName = targetScopeName;
        }
    }
}
