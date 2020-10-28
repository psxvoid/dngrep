using dngrep.core.Queries;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Console;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace dngrep.tool
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Should be fixed during further development.")]
        static async Task Main(string[] args)
        {
            using var container = new Container(x =>
            {
                x.Scan(o =>
                {
                    o.TheCallingAssembly();
                    o.AssemblyContainingType<SyntaxTreeQuery>();
                });

                x.AddSingleton(Parser.Default());
            });

            var pipeline = container.GetInstance<GrepCommandLinePipeline>();

            await pipeline.ParseArgsAndRun(args).ConfigureAwait(false);
        }
    }
}
