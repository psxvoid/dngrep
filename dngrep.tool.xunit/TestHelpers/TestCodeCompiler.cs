using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.tool.xunit.TestHelpers
{
    public static class TestCompiler
    {
        private static readonly Random RandomGenerator = new Random();

        public static CSharpCompilation Compile(string sourceCode)
        {
            var sourceText = SourceText.From(sourceCode);
            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText);

            int assemblyPostfixVersion = RandomGenerator.Next();

            return CSharpCompilation.Create($"Test{assemblyPostfixVersion}", new[] { syntaxTree });
        }
    }
}
