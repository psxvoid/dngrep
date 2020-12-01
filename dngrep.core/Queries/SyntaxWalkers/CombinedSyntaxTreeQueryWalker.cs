using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;

namespace dngrep.core.Queries.SyntaxWalkers
{
    public class CombinedSyntaxTreeQueryWalker : SyntaxTreeQueryWalkerBase<CombinedSyntaxNode>
    {
        public CombinedSyntaxTreeQueryWalker(SyntaxTreeQuery query) : base(query)
        {
        }

        protected override CombinedSyntaxNode CreateResultFromNode(SyntaxNode node)
        {
            return new CombinedSyntaxNode(node);
        }
    }
}
