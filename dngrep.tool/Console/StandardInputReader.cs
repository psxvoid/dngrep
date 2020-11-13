using System;
using System.IO;
using System.Threading.Tasks;
using dngrep.tool.Abstractions.System;

namespace dngrep.tool.Console
{
    public interface IStandardInputReader
    {
        bool IsInputRedirected();
        Task<string> ReadAsStringAsync();
    }

    public class StandardInputReader : IStandardInputReader
    {
        private readonly IStandardInputConsole console;

        public StandardInputReader(IStandardInputConsole console)
        {
            this.console = console;
        }

        public bool IsInputRedirected()
        {
            return this.console.IsInputRedirected;
        }

        public async Task<string> ReadAsStringAsync()
        {
            if (!this.console.IsInputRedirected)
            {
                throw new InvalidOperationException(
                    "Only read input stream when it's redirected, else use arguments.");
            }

            using var stream = this.console.OpenStandardInput();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
