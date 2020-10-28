using System.Threading.Tasks;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Core;
using dngrep.tool.Core.Options;

namespace dngrep.tool.Console
{
    public class GrepCommandLinePipeline
    {
        private readonly IParser parser;
        private readonly IProjectGrep grep;

        public GrepCommandLinePipeline(IParser parser, IProjectGrep grep)
        {
            this.parser = parser;
            this.grep = grep;
        }

        public async Task ParseArgsAndRun(string[] args)
        {
            await this.parser.ParseArguments<GrepOptions>(args)
                .WithParsedAsync(this.grep.FolderAsync).ConfigureAwait(false);
        }
    }
}
