using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace dngrep.tool.Abstractions.CodeAnalysis.CSharp
{
    public interface ICSharpSyntaxTreeStatic
    {
        SyntaxTree ParseText(string sourceCode);
    }

    public class CSharpSyntaxTreeStatic : ICSharpSyntaxTreeStatic
    {
        public SyntaxTree ParseText(string sourceCode)
        {
            return CSharpSyntaxTree.ParseText(sourceCode);
        }
    }
}
