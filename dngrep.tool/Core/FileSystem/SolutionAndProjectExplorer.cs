using System.IO.Abstractions;
using static dngrep.tool.Core.FileSystem.SolutionAndProjectExplorer;

namespace dngrep.tool.Core.FileSystem
{
    public interface ISolutionAndProjectExplorer
    {
        (PathKind kind, string path) GetSolutionOrProject(string path);
    }

    public class SolutionAndProjectExplorer : ISolutionAndProjectExplorer
    {
        public enum PathKind
        {
            None,
            Solution,
            Project
        }

        private readonly IFileSystem fs;

        public SolutionAndProjectExplorer(IFileSystem fs)
        {
            this.fs = fs;
        }

        public (PathKind kind, string path) GetSolutionOrProject(string path)
        {
            string[] solutions = this.fs.Directory.GetFiles(path, "*.sln");
            string[]? projects;

            if (solutions.Length > 0)
            {
                return (PathKind.Solution, solutions[0]);
            }
            else
            {
                projects = this.fs.Directory.GetFiles(path, "*.csproj");
            }

            if (projects != null && projects.Length > 0)
            {
                return (PathKind.Project, projects[0]);
            }

            return (PathKind.None, string.Empty);
        }
    }
}
