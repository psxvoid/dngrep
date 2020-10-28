using CommandLine;
using dngrep.core.Queries.Specifiers;

namespace dngrep.tool.Core.Options
{
    public class GrepOptions
    {
        [Option(
            't',
            "target",
            Default = QueryTarget.Any,
            Required = false,
            HelpText = "The type of the entity to search for (e.g. any, class, etc)")]
        public QueryTarget? Target { get; set; }

        [Option(
            's',
            "scope",
            Default = QueryTargetScope.None,
            Required = false,
            HelpText = "The type, containing the target (e.g. namespace, class, etc)")]
        public QueryTargetScope? Scope { get; set; }
    }
}
