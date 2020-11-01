﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQuery
    {
        public Type? TargetType { get; }
        public IEnumerable<string>? TargetNameContains { get; }
        public IEnumerable<string>? TargetNameExcludes { get; }
        public IReadOnlyCollection<SyntaxKind> TargetAccessModifiers { get; }

        public Type? ScopeType { get; }
        public IEnumerable<string>? TargetScopeContains { get; }

        public bool EnableRegex { get; }

        internal SyntaxTreeQuery(
            Type? targetType,
            IReadOnlyCollection<SyntaxKind> targetAccessModifiers,
            Type? scopeType,
            IEnumerable<string>? targetNameContains,
            IEnumerable<string>? targetNameExcludes,
            IEnumerable<string>? targetScopeContains,
            bool enableRegex)
        {
            this.TargetType = targetType;
            this.TargetNameContains = targetNameContains;
            this.TargetNameExcludes = targetNameExcludes;
            this.TargetAccessModifiers = targetAccessModifiers;
            this.ScopeType = scopeType;
            this.TargetScopeContains = targetScopeContains;
            this.EnableRegex = enableRegex;
        }
    }
}
