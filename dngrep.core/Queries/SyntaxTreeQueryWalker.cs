using dngrep.core.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;

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
                if (this.query.TargetScopeName != null && node.GetIdentifierName().Contains(this.query.TargetScopeName))
                {
                    this.scope = node;
                }
                else if (this.query.TargetScopeName == null)
                {
                    this.scope = node;
                }
            }

            if (this.query.TargetType == nodeType)
            {
                if (this.query.ScopeType != null
                    && this.scope != null
                    && (string.IsNullOrWhiteSpace(this.query.TargetName) || node.GetIdentifierName().Contains(this.query.TargetName))
                    && HasParent(node, this.scope))
                {
                    this.results.Add(node);
                }
                else if (this.query.ScopeType != null && string.IsNullOrWhiteSpace(this.query.TargetScopeName))
                {
                    this.results.Add(node);
                }
            }

            base.DefaultVisit(node);
        }

        private static bool HasParent(SyntaxNode node, SyntaxNode parent)
        {
            while (node.Parent != null)
            {
                if (ReferenceEquals(node.Parent, parent))
                {
                    return true;
                }

                node = node.Parent;
            }

            return false;
        }
    }
}
