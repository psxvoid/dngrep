using System.Threading.Tasks;
using dngrep.tool.Abstractions.Build;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Core;
using dngrep.tool.Core.Options;

namespace dngrep.tool.Console
{
    public class GrepCommandLinePipeline
    {
        private readonly IParser parser;
        private readonly IProjectGrep grep;
        private readonly IMSBuildLocator buildLocator;

        public GrepCommandLinePipeline(
            IParser parser,
            IProjectGrep grep,
            IMSBuildLocator buildLocator)
        {
            this.parser = parser;
            this.grep = grep;
            this.buildLocator = buildLocator;
        }

        public async Task ParseArgsAndRun(string[] args)
        {
            this.buildLocator.RegisterDefaults();

            await this.parser.ParseArguments<GrepOptions>(args)
                .WithParsedAsync(this.grep.FolderAsync).ConfigureAwait(false);
        }
    }
}
