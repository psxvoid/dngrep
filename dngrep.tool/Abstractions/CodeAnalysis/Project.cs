﻿using System.Threading.Tasks;
using dngrep.tool.Abstractions.CodeAnalysis.CSharp;
using MSCompilation = Microsoft.CodeAnalysis.Compilation;
using MSCSharpCompilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation;
using MSProject = Microsoft.CodeAnalysis.Project;

namespace dngrep.tool.Abstractions.CodeAnalysis
{
    public interface IProject
    {
        Task<ICompilation?> GetCompilationAsync();
    }

    public class Project : IProject
    {
        public MSProject MSProject { get; }

        public Project(MSProject project)
        {
            this.MSProject = project;
        }

        public async Task<ICompilation?> GetCompilationAsync()
        {
            MSCompilation? compilation = await this.MSProject.GetCompilationAsync()
                .ConfigureAwait(false);

            return compilation switch
            {
                MSCSharpCompilation cSharpCompilation => new CSharpCompilation(cSharpCompilation),
                null => null,
                _ => new Compilation(compilation)
            };
        }
    }
}
