using System.Collections.Generic;
using dngrep.core.Queries.Specifiers;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries
{
    /// <summary>
    /// Describes a <see cref="SyntaxTreeQuery"/> that should be performed on a <see cref="SyntaxTree"/>.
    /// </summary>
    public class SyntaxTreeQueryDescriptor
    {
        public QueryTarget Target { get; }
        public QueryAccessModifier AccessModifier { get; }
        public QueryTargetScope Scope { get; }
        public IEnumerable<string>? TargetNameContains { get; }
        public IEnumerable<string>? TargetNameExcludes { get; }
        public string? QueryTargetScopeName { get; }
        public bool EnableRegex { get; }

        public SyntaxTreeQueryDescriptor(
            QueryTarget queryTarget,
            QueryAccessModifier accessModifier,
            QueryTargetScope targetScope,
            IEnumerable<string>? targetNameContains,
            IEnumerable<string>? targetNameExcludes,
            string? queryTargetScopeName,
            bool enableRegex = false)
        {
            this.Target = queryTarget;
            this.AccessModifier = accessModifier;
            this.Scope = targetScope;
            this.TargetNameContains = targetNameContains;
            this.TargetNameExcludes = targetNameExcludes;
            this.QueryTargetScopeName = queryTargetScopeName;
            this.EnableRegex = enableRegex;
        }
    }
}
