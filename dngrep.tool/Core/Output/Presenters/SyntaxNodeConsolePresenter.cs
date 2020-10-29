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
                string? name = options.ShowFullName ?? false ?
                    node.TryGetFullName() : node.TryGetIdentifierName();

                if (name == null) continue;

                this.console.WriteLine(name);

                if (options.ShowFilePath ?? false)
                {
                    string? path = node.GetLocation()?.SourceTree?.FilePath;

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        this.console.WriteLine($"\t{path}");
                    }
                }

                if (options.ShowPosition ?? false)
                {
                    LinePosition position = node.GetLocation().GetLineSpan().StartLinePosition;
                    this.console.WriteLine($"\tLn: {position.Line}, Ch: {position.Character}");
                }
            }
        }
    }
}
