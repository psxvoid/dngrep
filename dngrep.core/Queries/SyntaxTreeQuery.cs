using System;
using System.Collections.Generic;
using System.Text;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQuery
    {
        public Type TargetType { get; }
        public Type? ScopeType { get; }

        public string? TargetName { get; }
        public string? TargetScopeName { get; }

        internal SyntaxTreeQuery(Type targetType, Type? scopeType, string? targetName, string? targetScopeName)
        {
            this.ScopeType = scopeType;
            this.TargetType = targetType;
            this.TargetName = targetName;
            this.TargetScopeName = targetScopeName;
        }
    }
}
