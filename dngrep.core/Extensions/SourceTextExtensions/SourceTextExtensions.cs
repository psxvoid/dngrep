using System;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.Extensions.SourceTextExtensions
{
    public static class SourceTextExtensions
    {
        public static TextSpan GetLineSpanAtPosition(this SourceText source, int line, int charStart)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            if (line < 0 || line >= source.Lines.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(line),
                    "Specified line number is out of range from the source text.");
            }

            TextSpan lineSpan = source.Lines[line].Span;

            if (charStart < 0 || charStart >= lineSpan.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(line),
                    "Specified char position is out of range from the source text.");
            }

            return new TextSpan(lineSpan.Start + charStart, lineSpan.Length - charStart);
        }
    }
}
