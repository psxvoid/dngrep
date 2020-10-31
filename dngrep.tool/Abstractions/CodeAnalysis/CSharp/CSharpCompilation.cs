using MSCompilation = Microsoft.CodeAnalysis.Compilation;
using MSCSharpCompilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation;

namespace dngrep.tool.Abstractions.CodeAnalysis.CSharp
{
    public interface ICSharpCompilation : ICompilation
    {
        MSCSharpCompilation MSCSharpCompilation { get; }
    }

    public sealed class CSharpCompilation : ICSharpCompilation
    {
        public MSCSharpCompilation MSCSharpCompilation { get; }

        MSCompilation ICompilation.MSCompilation => this.MSCSharpCompilation;

        public CSharpCompilation(MSCSharpCompilation compilation)
        {
            this.MSCSharpCompilation = compilation;
        }
    }
}
