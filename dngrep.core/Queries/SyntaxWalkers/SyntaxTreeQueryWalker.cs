using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries
{
    public class SyntaxTreeQueryWalker : SyntaxTreeQueryWalkerBase<SyntaxNode>
    {
        public SyntaxTreeQueryWalker(SyntaxTreeQuery query) : base(query)
        {
        }

        protected override SyntaxNode CreateResultFromNode(SyntaxNode node)
        {
            return node;
        }
    }
}
