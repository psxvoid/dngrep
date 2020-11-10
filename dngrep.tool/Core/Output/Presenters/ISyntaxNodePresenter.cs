using System.Collections.Generic;
using dngrep.tool.Core.Options;
using Microsoft.CodeAnalysis;

namespace dngrep.tool.Core.Output.Presenters
{
    public interface ISyntaxNodePresenter
    {
        void ProduceOutput(IEnumerable<SyntaxNode> nodes, GrepOptions options);
    }
}
