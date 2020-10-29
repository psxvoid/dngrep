using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQuery
    {
        public Type? TargetType { get; }
        public IEnumerable<string>? TargetNameContains { get; }
        public IReadOnlyCollection<SyntaxKind> TargetAccessModifiers { get; }

        public Type? ScopeType { get; }
        public string? TargetScopeName { get; }

        internal SyntaxTreeQuery(
            Type? targetType,
            IReadOnlyCollection<SyntaxKind> targetAccessModifiers,
            Type? scopeType,
            IEnumerable<string>? targetNameContains,
            string? targetScopeName)
        {
            this.TargetType = targetType;
            this.TargetNameContains = targetNameContains;
            this.TargetAccessModifiers = targetAccessModifiers;
            this.ScopeType = scopeType;
            this.TargetScopeName = targetScopeName;
        }
    }
}
