using CommandLine;
using dngrep.core.Queries.Specifiers;

namespace dngrep.tool.Core.Options
{
    public class GrepOptions
    {
        [Option('t',
            Required = true,
            HelpText = "The type of the entity to search for (e.g. any, class, etc)")]
        public QueryTarget Target { get; set; }
    }
}
