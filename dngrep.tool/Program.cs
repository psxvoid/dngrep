using System.IO.Abstractions;
using System.Threading.Tasks;
using dngrep.core.Queries;
using dngrep.tool.Abstractions.CommandLine;
using dngrep.tool.Console;
using dngrep.tool.Core;
using Lamar;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace dngrep.tool
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var container = new Container(x =>
            {
                x.Scan(o =>
                {
                    o.TheCallingAssembly();
                    o.AssemblyContainingType<SyntaxTreeQuery>();
                    o.AssemblyContainingType<Grep>();

                    o.SingleImplementationsOfInterface();
                    o.RegisterConcreteTypesAgainstTheFirstInterface();
                });

                x.AddSingleton(Parser.Default());
                x.AddSingleton<IFileSystem, FileSystem>();
                x.AddTransient<CSharpSyntaxWalker, SyntaxTreeQueryWalker>();
            });

            var pipeline = container.GetInstance<GrepCommandLinePipeline>();

            await pipeline.ParseArgsAndRun(args).ConfigureAwait(false);
        }
    }
}
