﻿using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQueryWalker : CSharpSyntaxWalker
    {
        private readonly List<SyntaxNode> results = new List<SyntaxNode>();
        private readonly SyntaxTreeQuery query;

        private SyntaxNode? scope;

        public IReadOnlyCollection<SyntaxNode> Results => this.results;

        public SyntaxTreeQueryWalker(SyntaxTreeQuery query)
        {
            this.query = query;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();

            if (this.query.ScopeType != null && nodeType == this.query.ScopeType)
            {
                if (string.IsNullOrWhiteSpace(this.query.TargetScopeName)
                 || node.GetIdentifierName().Contains(this.query.TargetScopeName))
                {
                    this.scope = node;
                }
            }

            if (this.query.TargetType == nodeType || this.query.TargetType == null)
            {
                if (
                    // match target name, OR boolean query
                    (this.query.TargetNameContains == null
                        || !this.query.TargetNameContains.Any()
                        || this.query.TargetNameContains.Any(
                            x => node.GetIdentifierName().Contains(x)))
                    // exclude target name, OR boolean query
                    && (this.query.TargetNameExcludes == null
                        || !this.query.TargetNameExcludes.Any()
                        || !this.query.TargetNameExcludes.Any(
                            x => node.GetIdentifierName().Contains(x)))
                    // match scope type
                    && (this.query.ScopeType == null || this.scope != null && node.HasParent(this.scope))
                    // match access modifiers
                    && (this.query.TargetAccessModifiers == null || this.query.TargetAccessModifiers.Count == 0
                        || node.ChildTokens().Select(x => x.Kind()).Intersect(this.query.TargetAccessModifiers).Count()
                            == this.query.TargetAccessModifiers.Count))
                {
                    this.results.Add(node);
                }
            }

            base.DefaultVisit(node);
        }
    }
}
