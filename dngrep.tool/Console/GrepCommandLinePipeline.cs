using System.Threading.Tasks;
using dngrep.tool.Abstractions.Build;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Core;
using dngrep.tool.Core.Exceptions;
using dngrep.tool.Core.Options;

namespace dngrep.tool.Console
{
    public class GrepCommandLinePipeline
    {
        private readonly IParser parser;
        private readonly IProjectGrep grep;
        private readonly IMSBuildLocator buildLocator;
        private readonly IStringConsole console;
        private readonly IStandardInputReader inputReader;

        public GrepCommandLinePipeline(
            IParser parser,
            IProjectGrep grep,
            IMSBuildLocator buildLocator,
            IStringConsole console,
            IStandardInputReader inputReader)
        {
            this.parser = parser;
            this.grep = grep;
            this.buildLocator = buildLocator;
            this.console = console;
            this.inputReader = inputReader;
        }

        public async Task ParseArgsAndRun(string[] args)
        {
            IParserResult<GrepOptions> parseResult = this.parser.ParseArguments<GrepOptions>(args);

            try
            {
                if (this.inputReader.IsInputRedirected())
                {
                    string text = await this.inputReader.ReadAsStringAsync().ConfigureAwait(false);
                    await parseResult.WithParsedAsync((o) => this.grep.TextAsSyntaxTree(o, text))
                        .ConfigureAwait(false);
                }
                else
                {
                    this.buildLocator.RegisterDefaults();
                    await parseResult.WithParsedAsync(this.grep.FolderAsync).ConfigureAwait(false);
                }
            }
            catch (GrepException grepException)
            {
                this.console.WriteLine(grepException.Message);
            }
        }
    }
}
