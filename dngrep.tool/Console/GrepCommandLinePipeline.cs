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
        private readonly IConsole console;

        public GrepCommandLinePipeline(
            IParser parser,
            IProjectGrep grep,
            IMSBuildLocator buildLocator,
            IConsole console)
        {
            this.parser = parser;
            this.grep = grep;
            this.buildLocator = buildLocator;
            this.console = console;
        }

        public async Task ParseArgsAndRun(string[] args)
        {
            this.buildLocator.RegisterDefaults();

            try
            {
                await this.parser.ParseArguments<GrepOptions>(args)
                    .WithParsedAsync(this.grep.FolderAsync).ConfigureAwait(false);
            }
            catch (GrepException grepException)
            {
                this.console.WriteLine(grepException.Message);
            }
        }
    }
}
