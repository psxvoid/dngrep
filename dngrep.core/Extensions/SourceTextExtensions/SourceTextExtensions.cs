using System;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.Extensions.SourceTextExtensions
{
    public static class SourceTextExtensions
    {
        /// <summary>
        /// Get <see cref="TextSpan"/> of length one (or zero for an empty line)
        /// for a specified line and char position in a source text. For example,
        /// it can be used to get calculate a cursor location in the source text.
        /// </summary>
        /// <param name="source">
        /// The source text where the line is belongs.
        /// </param>
        /// <param name="line">
        /// Zero-based line number in the source text for which a single character's
        /// span should be retrieved.
        /// </param>
        /// <param name="charStart">
        /// Zero-based character number in the source text at the specified line for
        /// which a span should be retrieved.
        /// </param>
        /// <returns>
        /// The span of a single character located at the specified line and position.
        /// The span contains exact location of the character.
        /// </returns>
        public static TextSpan GetSingleCharSpan(this SourceText source, int line, int charStart)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            if (line < 0 || line >= source.Lines.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(line),
                    "Specified line number is out of range from the source text.");
            }

            TextSpan lineSpan = source.Lines[line].Span;

            if (charStart < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(line),
                    "Specified char position is out of range from the source text.");
            }

            if (charStart >= lineSpan.Length)
            {
                charStart = lineSpan.Length - 1;
            }

            int spanLength = 1;

            if (lineSpan.Length == 0)
            {
                charStart = 0;
                spanLength = 0;
            }

            return new TextSpan(lineSpan.Start + charStart, spanLength);
        }
    }
}
