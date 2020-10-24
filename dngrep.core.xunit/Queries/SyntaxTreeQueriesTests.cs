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
                public class HarryPotterBook
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

            public class GetAnyMethodsInAnyClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetAnyMethodsInAnyClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Class,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetFourMethods()
                {
                    Assert.Equal(4, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodNames()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Annotate1", names);
                    Assert.Contains("Read1", names);
                    Assert.Contains("Annotate2", names);
                    Assert.Contains("Read2", names);
                }
            }
        }

        public static class AmbigiusMethodsInClassAndStruct
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
            
            public class GetMethodsInAnyClass
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodsInAnyClass()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Class,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetTwoMatches()
                {
                    Assert.Equal(2, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodsFromMatchingClass()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Annotate2", names);
                    Assert.Contains("Read2", names);
                }
            }
            
            public class GetMethodsInAnyStruct
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodsInAnyStruct()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Struct,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetTwoMatches()
                {
                    Assert.Equal(2, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodsFromMatchingClass()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Annotate1", names);
                    Assert.Contains("Read1", names);
                }
            }
            
            public class GetMethodsInSpecifiedClass
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodsInSpecifiedClass()
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
                public void ShouldGetTwoMatches()
                {
                    Assert.Equal(2, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodsFromMatchingClass()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Annotate2", names);
                    Assert.Contains("Read2", names);
                }
            }
            
            public class GetMethodsInSpecifiedStruct
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodsInSpecifiedStruct()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Struct,
                        null,
                        "HarryPotterBook"
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetTwoMatch()
                {
                    Assert.Equal(2, this.results.Count);
                }

                [Fact]
                public void ShouldGetMethodsFromMatchingClass()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Annotate1", names);
                    Assert.Contains("Read1", names);
                }
            }

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

            public class GetMethodWithNameContainingStringInStructWithName
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodWithNameContainingStringInStructWithName()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Struct,
                        "Read",
                        "HarryPotterBook"
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
                public void ShouldGetMethodFromMatchingStruct()
                {
                    Assert.Equal("Read1", this.results.First().GetIdentifierName());
                }
            }
        }
    }
}
