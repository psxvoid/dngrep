using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace dngrep.core.xunit.Queries
{
    /// <summary>
    /// Integration tests for SyntaxTree Queries.
    /// </summary>
    public static class SyntaxTreeQueriesTests
    {
        public static class AmbigiusMethodsInDifferentClasses
        {
            private const string SourceCode = @"
                public struct HarryPotterBook
                {
                    public void Annotate1()
                    {
                    }

                    public void Read1()
                    {
                    }
                }

                public class LordOfTheRingsBook
                {
                    public void Annotate2()
                    {
                    }

                    public void Read2()
                    {
                    }
                }
            ";

            public class GetMethodWithNameContainingStringInClassWithName
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodWithNameContainingStringInClassWithName()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Class,
                        "Read",
                        "LordOfTheRingsBook"
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetSingleMatch()
                {
                    Assert.Equal(1, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodFromMatchingClass()
                {
                    Assert.Equal("Read2", this.results.First().GetIdentifierName());
                }
            }
            
            public class GetMethodsInClassWithName
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodsInClassWithName()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Class,
                        null,
                        "LordOfTheRingsBook"
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetTwoMethods()
                {
                    Assert.Equal(2, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodNamesFromMatchingClass()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Annotate2", names);
                    Assert.Contains("Read2", names);
                }
            }
        }
    }
}
