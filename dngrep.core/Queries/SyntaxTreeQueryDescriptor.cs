using System.Collections.Generic;
using System.Linq;
using dngrep.core.Queries.Specifiers;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries
{
    /// <summary>
    /// Describes a <see cref="SyntaxTreeQuery"/> that should be performed on a <see cref="SyntaxTree"/>.
    /// </summary>
    public class SyntaxTreeQueryDescriptor
    {
        public QueryTarget Target { get; set; } = QueryTarget.Any;
        public QueryAccessModifier AccessModifier { get; set; } = QueryAccessModifier.Any;
        public QueryTargetScope Scope { get; set; } = QueryTargetScope.None;
        public IEnumerable<string> TargetNameContains { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> TargetNameExcludes { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> TargetScopeContains { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> TargetScopeExcludes { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> TargetPathContains { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> TargetPathExcludes { get; set; } = Enumerable.Empty<string>();

        public bool EnableRegex { get; set; }
    }
}
