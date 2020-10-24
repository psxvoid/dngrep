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
        public class AmbigiusMethodsInDifferentClasses_GetMethodWithNameContainingStringInClassWithName
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

            private readonly IReadOnlyCollection<SyntaxNode> results;

            public AmbigiusMethodsInDifferentClasses_GetMethodWithNameContainingStringInClassWithName()
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
            public void Results_ShouldGetSingleMatch()
            {
                Assert.Equal(1, this.results.Count);
            }

            [Fact]
            public void Results_ShouldMatchSecondClassMethodName()
            {
                Assert.Equal("Read2", this.results.First().GetIdentifierName());
            }
        }
    }
}
