using MSCompilation = Microsoft.CodeAnalysis.Compilation;

namespace dngrep.tool.Abstractions.CodeAnalysis
{
    public interface ICompilation
    {
        MSCompilation MSCompilation { get; }
    }

    public class Compilation : ICompilation
    {
        public MSCompilation MSCompilation { get; }

        public Compilation(MSCompilation compilation)
        {
            this.MSCompilation = compilation;
        }
    }
}
