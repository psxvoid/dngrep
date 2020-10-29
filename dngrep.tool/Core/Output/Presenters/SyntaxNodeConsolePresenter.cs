using System;
using System.Collections.Generic;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Core.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.tool.Core.Output.Presenters
{
    public interface ISyntaxNodePresenter
    {
        void ProduceOutput(IEnumerable<SyntaxNode> nodes, GrepOptions options);
    }

    public class ConsoleSyntaxNodePresenter : ISyntaxNodePresenter
    {
        private readonly IConsole console;

        private const string PositionSeparator = ":";
        private const string FileAndPositionPrefix = "at ";

        public ConsoleSyntaxNodePresenter(IConsole console)
        {
            this.console = console;
        }

        public void ProduceOutput(IEnumerable<SyntaxNode> nodes, GrepOptions options)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            foreach (var node in nodes)
            {
                string? name = options.ShowFullName ?? false
                    ? node.TryGetFullName(options.HideNamespaces ?? false)
                    : node.TryGetIdentifierName();

                if (name == null) continue;

                this.console.WriteLine(name);

                bool isPathShown = options.ShowFilePath ?? false;
                bool isPositionShown = options.ShowPosition ?? false;

                if (isPathShown)
                {
                    string? path = node.GetLocation()?.SourceTree?.FilePath;

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        string newLine = isPositionShown ? string.Empty : Environment.NewLine;
                        this.console.Write($"\t{FileAndPositionPrefix}{path}{newLine}");
                    }
                }

                if (options.ShowPosition ?? false)
                {
                    LinePosition position = node.GetLocation().GetLineSpan().StartLinePosition;
                    string separator = isPathShown ? PositionSeparator : FileAndPositionPrefix;
                    string tabulator = isPathShown ? string.Empty : "\t";
                    this.console.WriteLine(
                        $"{tabulator}{separator}line {position.Line}, char {position.Character}");
                }
            }
        }
    }
}
