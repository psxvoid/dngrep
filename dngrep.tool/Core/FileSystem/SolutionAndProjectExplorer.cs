using System.IO;

namespace dngrep.tool.Core.FileSystem
{
    public static class SolutionAndProjectExplorer
    {
        public enum PathKind
        {
            None,
            Solution,
            Project
        }

        public static (PathKind kind, string path) GetSolutionOrProject(string path)
        {
            string[] solutions = Directory.GetFiles(path, "*.sln");
            string[]? projects;

            if (solutions.Length > 0)
            {
                return (PathKind.Solution, solutions[0]);
            }
            else
            {
                projects = Directory.GetFiles(path, "*.csproj");
            }

            if (projects != null && projects.Length > 0)
            {
                return (PathKind.Project, projects[0]);
            }

            return (PathKind.None, string.Empty);
        }
    }
}
