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

        public static class AmbigiusMethodsInClassesPublicAndPrivate
        {
            private const string SourceCode = @"
                public class Harry
                {
                    public void Walk1()
                    {
                    }

                    public void Jump1()
                    {
                    }

                    private void UseCloak()
                    {
                    }
                }

                public class Sam
                {
                    private void Walk2()
                    {
                    }

                    private void Jump2()
                    {
                    }
                }
            ";

            public class GetPrivateMethodsInAnyClass
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetPrivateMethodsInAnyClass()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Private,
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
                public void ShouldGetThreeMatches()
                {
                    Assert.Equal(3, this.results.Count);
                }

                [Fact]
                public void ShouldGetPrivateMethodsFromClasses()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("UseCloak", names);
                    Assert.Contains("Walk2", names);
                    Assert.Contains("Jump2", names);
                }
            }

            public class GetPrivateMethodsInSpecificClass
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetPrivateMethodsInSpecificClass()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Private,
                        QueryTargetScope.Class,
                        null,
                        "Harry"
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
                public void ShouldGetMethodsFromMatchingClass()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("UseCloak", names);
                }
            }

            public class GetPublicMethodsInSpecificClass
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetPublicMethodsInSpecificClass()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Public,
                        QueryTargetScope.Class,
                        null,
                        "Harry"
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
                    Assert.Contains("Walk1", names);
                    Assert.Contains("Jump1", names);
                }
            }
        }

        public static class PublicClassAndInternalClassAndInternalProtectedClass
        {
            private const string SourceCode = @"
                public class Warrior
                {
                }
                
                internal class PoliceMan
                {
                }

                protected internal class Spy
                {
                }
            ";

            public class GetPublicClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetPublicClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Class,
                        QueryAccessModifier.Public,
                        QueryTargetScope.None,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetSingleMatch()
                {
                    Assert.Single(this.results);
                }

                [Fact]
                public void ShouldGetPublicClassName()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Warrior", names);
                }
            }

            public class GetProtectedInternalClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetProtectedInternalClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Class,
                        QueryAccessModifier.ProtectedInternal,
                        QueryTargetScope.None,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetSingleMatch()
                {
                    Assert.Single(this.results);
                }

                [Fact]
                public void ShouldGetProtectedInternalClassName()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Spy", names);
                }
            }

            public class GetProtectedClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetProtectedClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Class,
                        QueryAccessModifier.Protected,
                        QueryTargetScope.None,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetSingleMatch()
                {
                    Assert.Single(this.results);
                }

                [Fact]
                public void ShouldGetProtectedInternalClassName()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Spy", names);
                }
            }

            public class GetInternalClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetInternalClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Class,
                        QueryAccessModifier.Internal,
                        QueryTargetScope.None,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetSingleMatch()
                {
                    Assert.Equal(2, this.results.Count);
                }

                [Fact]
                public void ShouldGetInternalClassName()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("PoliceMan", names);
                }

                [Fact]
                public void ShouldGetProtectedInternalClassName()
                {
                    IEnumerable<string>? names = this.results.Select(x => x.GetIdentifierName());
                    Assert.Contains("Spy", names);
                }
            }
        }

        public static class EachSupportedType
        {
            private const string SourceCode = @"
                using System;

                namespace Zoo
                {
                    public interface IAnimal {
                    }

                    public enum WeatherState {
                        Sunny = 0
                    }

                    public struct Sun {
                    }

                    public class Cat : IAnimal
                    {
                        private int lives = 9;
                        public int RemainingLives => this.lives;

                        public event EventHandler<EventArgs> OnWakeUp;

                        public string GetPurrText()
                        {
                            var catName = ""Felix"";
                            return catName + "" is purring"";
                        }
                    }
                }
            ";

            public class GetAnyTargetWithAnyModifierInAnyScope
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetAnyTargetWithAnyModifierInAnyScope()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Any,
                        QueryAccessModifier.Any,
                        QueryTargetScope.None,
                        null,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetAllSyntaxNodes()
                {
                    Assert.Equal(47, this.results.Count);
                }

                [Fact]
                public void ShouldGetNamespace()
                {
                    Assert.Contains("Zoo", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetClass()
                {
                    Assert.Contains("Cat", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetStruct()
                {
                    Assert.Contains("Sun", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetEnum()
                {
                    Assert.Contains("WeatherState", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetInterface()
                {
                    Assert.Contains("IAnimal", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetField()
                {
                    Assert.Contains("lives", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetProperty()
                {
                    Assert.Contains("RemainingLives", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetEvent()
                {
                    Assert.Contains("OnWakeUp", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetMethod()
                {
                    Assert.Contains("GetPurrText", this.results.Select(x => x.TryGetIdentifierName()));
                }

                [Fact]
                public void ShouldGetVariable()
                {
                    Assert.Contains("catName", this.results.Select(x => x.TryGetIdentifierName()));
                }
            }
        }
    }
}
