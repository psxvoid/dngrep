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
            'c',
            "contains",
            Required = false,
            HelpText = "A part of the target name (e.g. Foo) to filter for.")]
        public string? TargetName { get; set; }

        [Option(
            's',
            "scope",
            Default = QueryTargetScope.None,
            Required = false,
            HelpText = "The type, containing the target (e.g. namespace, class, etc)")]
        public QueryTargetScope? Scope { get; set; }

        [Option(
            'f',
            "show-full-name",
            Default = true,
            Required = false,
            HelpText = "When set to true, then output will contain full target name, "
            + "including namespace, etc (default=true).")]
        public bool? ShowFullName { get; set; }

        [Option(
            'p',
            "show-file-path",
            Default = true,
            Required = false,
            HelpText = "When set to true, then output will contain a file name, "
            + "containing the target. (default=true).")]
        public bool? ShowFilePath { get; set; }
        
        [Option(
            'l',
            "show-location",
            Default = true,
            Required = false,
            HelpText = "When set to true, then output will contain a line and, "
            + "the character location of the target. (default=true).")]
        public bool? ShowPosition { get; set; }
    }
}
