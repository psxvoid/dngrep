using SystemConsole = System.Console;

namespace dngrep.tool.Abstractions.System
{
    public interface IConsole
    {
        void Write(string value);

        void WriteLine(string value);

        void WriteLine();
    }

    public class Console : IConsole
    {
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
    }
}
