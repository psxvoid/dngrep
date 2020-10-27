using CommandLine;
using dngrep.tool.Core;
using dngrep.tool.Core.Options;
using System.Threading.Tasks;

namespace dngrep.tool
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Should be fixed during further development.")]
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<GrepOptions>(args)
                   .WithParsedAsync(Grep.FolderAsync).ConfigureAwait(false);
        }
    }
}
