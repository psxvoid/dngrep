﻿using System.Collections.Generic;
using CommandLine;
using dngrep.core.Queries.Specifiers;
using dngrep.tool.Core.Output.Presenters;

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
            HelpText = "A part of the target name (e.g. Foo) to filter for. "
                + "Supports multiple space-separated values.")]
        public IEnumerable<string>? Contains { get; set; }

        [Option(
            'e',
            "exclude",
            Required = false,
            HelpText = "A part of the target name (e.g. Foo) to exclude from results. "
                + "Supports multiple space-separated values.")]
        public IEnumerable<string>? Exclude { get; set; }

        [Option(
            'r',
            "regexp",
            Default = false,
            Required = false,
            HelpText = "Enables passing regular expressions to contains and exclude "
            + "options. (default=false).")]
        public bool? EnableRegexp { get; set; }

        [Option(
            's',
            "scope",
            Default = QueryTargetScope.None,
            Required = false,
            HelpText = "The type, containing the target (e.g. namespace, class, etc)")]
        public QueryTargetScope? Scope { get; set; }

        [Option(
            'C',
            "scope-contains",
            Required = false,
            HelpText = "A part of the target scope name (e.g. Foo) to include in results. "
                + "Supports multiple space-separated values.")]
        public IEnumerable<string>? ScopeContains { get; set; }

        [Option(
            'E',
            "scope-exclude",
            Required = false,
            HelpText = "A part of the target scope name (e.g. Foo) to exclude from results. "
                + "Supports multiple space-separated values.")]
        public IEnumerable<string>? ScopeExclude { get; set; }

        [Option(
            'p',
            "path-contains",
            Required = false,
            HelpText = "A part of a file path of the target to include.")]
        public IEnumerable<string>? PathContains { get; set; }

        [Option(
            'P',
            "path-exclude",
            Required = false,
            HelpText = "A part of a file path of the target to exclude.")]
        public IEnumerable<string>? PathExclude { get; set; }

        [Option(
            'n',
            "show-full-name",
            Default = true,
            Required = false,
            HelpText = "When set to true, then output will contain full target name, "
            + "including namespace, etc (default=true).")]
        public bool? ShowFullName { get; set; }

        [Option(
            'l',
            "show-file-path",
            Default = true,
            Required = false,
            HelpText = "When set to true, then output will contain a file name (location), "
            + "containing the target. (default=true).")]
        public bool? ShowFilePath { get; set; }

        [Option(
            'L',
            "show-location",
            Default = true,
            Required = false,
            HelpText = "When set to true, then output will contain a line and, "
            + "the character location of the target. (default=true).")]
        public bool? ShowPosition { get; set; }

        [Option(
            "hide-namespaces",
            Default = false,
            Required = false,
            HelpText = "When set to true, then namespaces will be omitted when show-full-name "
            + "is used, ignored else (default=false).")]
        public bool? HideNamespaces { get; set; }

        [Option(
            'o',
            "output-type",
            Default = PresenterKind.Search,
            Required = false,
            HelpText = "How to output the type. By default instructs to print search results. "
                + "Can be set to \"statistics\" to only output search statistics. "
                + "It requires the same time to search but only shows a number of found nodes.")]
        public PresenterKind? OutputType { get; set; }
    }
}
