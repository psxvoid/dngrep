﻿using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxWalkers.MatchStrategies
{
    public class ScopedSyntaxNodeMatchStrategy : ISyntaxNodeMatchStrategy
    {
        public ScopedSyntaxNodeMatchStrategy(SyntaxTreeQuery query)
        {
            this.query = query;
        }

        private readonly SyntaxTreeQuery query;

        private SyntaxNode? scope;

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();
            string? nodeName = node.TryGetIdentifierName();

            if (this.query.HasScope && this.query.ScopeMatchers.Any(x => x.Match(node)))
            {
                this.scope = node;
            }

            // do not include nodes without the name in results
            return
                nodeName != null
                && (!this.query.HasTarget || this.query.TargetMatchers.Any(x => x.Match(node)))
                && (!this.query.HasScope || this.scope != null && node.HasParent(this.scope))
                && (!this.query.HasAccessModifiers
                    || this.query.AccessModifierMatchers.Any(x => x.Match(node)))
                && (!this.query.HasPathMatchers || this.query.PathMatchers.All(x => x.Match(node)));
        }
    }
}