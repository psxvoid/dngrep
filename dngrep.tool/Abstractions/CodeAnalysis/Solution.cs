using System.Collections.Generic;
using System.Linq;
using MSSolution = Microsoft.CodeAnalysis.Solution;

namespace dngrep.tool.Abstractions.CodeAnalysis
{
    public interface ISolution
    {
        IEnumerable<IProject> Projects { get; }
    }

    public class Solution : ISolution
    {
        public MSSolution MSSolution { get; }
        public IEnumerable<IProject> Projects => this.MSSolution.Projects.Select(x => new Project(x));

        public Solution(MSSolution solution)
        {
            this.MSSolution = solution;
        }
    }
}
