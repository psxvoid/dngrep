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
        public string? QueryTargetName { get; }
        public string? QueryTargetScopeName { get; }

        public SyntaxTreeQueryDescriptor(
            QueryTarget queryTarget,
            QueryAccessModifier accessModifier,
            QueryTargetScope targetScope,
            string? queryTargetName,
            string? queryTargetScopeName)
        {
            this.Target = queryTarget;
            this.AccessModifier = accessModifier;
            this.Scope = targetScope;
            this.QueryTargetName = queryTargetName;
            this.QueryTargetScopeName = queryTargetScopeName;
        }
    }
}
