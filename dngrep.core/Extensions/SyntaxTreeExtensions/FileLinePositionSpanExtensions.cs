using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace dngrep.core.Extensions.SyntaxTreeExtensions
{
    public static class FileLinePositionSpanExtensions
    {

        /// <summary>
        /// Gets the bounds of the <see cref="FileLinePositionSpan"/> in the source text.
        /// </summary>
        /// <param name="lineSpan">The line span that contains the source text bounds.</param>
        /// <returns>The bounds of the node in the source text.</returns>
        public static (int lineStart, int lineEnd, int charStart, int charEnd)
            GetSourceTextBounds(this FileLinePositionSpan lineSpan)
        {
            LinePosition startPosition = lineSpan.StartLinePosition;
            LinePosition endPosition = lineSpan.EndLinePosition;

            return (
                startPosition.Line,
                endPosition.Line,
                startPosition.Character,
                endPosition.Character);
        }
    }
}
