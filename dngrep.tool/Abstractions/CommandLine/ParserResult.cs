using System;
using System.Threading.Tasks;
using CommandLine;
using cmd = CommandLine;

namespace dngrep.tool.Abstractions.CommandLine
{
    public interface IParserResult<T>
    {
        Task<IParserResult<T>> WithParsedAsync(Func<T, Task> action);
    }

    public class ParserResult<T> : IParserResult<T>
    {
        private readonly cmd.ParserResult<T> result;

        public ParserResult(cmd.ParserResult<T> result)
        {
            this.result = result;
        }

        public async Task<IParserResult<T>> WithParsedAsync(Func<T, Task> action)
        {
            return new ParserResult<T>(
                await ParserResultExtensions.WithParsedAsync(this.result, action)
                    .ConfigureAwait(false));
        }
    }
}
