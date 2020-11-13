using System.IO;
using SystemConsole = System.Console;

namespace dngrep.tool.Abstractions.System
{
    public interface IStringConsole
    {
        void Write(string value);

        void WriteLine(string value);

        void WriteLine();
    }

    public interface IStandardInputConsole
    {
        bool IsInputRedirected { get; }

        Stream OpenStandardInput();
    }

    public class Console : IStringConsole, IStandardInputConsole
    {
        public bool IsInputRedirected => SystemConsole.IsInputRedirected;

        public void Write(string value)
        {
            SystemConsole.Write(value);
        }

        public void WriteLine(string value)
        {
            SystemConsole.WriteLine(value);
        }

        public void WriteLine()
        {
            SystemConsole.WriteLine();
        }

        public Stream OpenStandardInput()
        {
            return SystemConsole.OpenStandardInput();
        }
    }
}
