using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxWalkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        TargetNameContains = new[] { "Read" },
                        TargetScopeContains = new[] { "LordOfTheRingsBook" },
                    };

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

            public class GetMethodWithNameContainingStringInClassWithNameRegex
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodWithNameContainingStringInClassWithNameRegex()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        TargetNameContains = new[] { "Read" },
                        TargetScopeContains = new[] { "o.{4,4}Book" },
                        EnableRegex = true,
                    };

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
                    Assert.Equal("Read1", this.results.First().GetIdentifierName());
                }
            }

            public class GetMethodWithNameContainingStringInClassWithWithoutNameRegex
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetMethodWithNameContainingStringInClassWithWithoutNameRegex()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        TargetNameContains = new[] { "Read" },
                        TargetScopeContains = new[] { "o.*Book" },
                        TargetScopeExcludes = new[] { "[tT]e" },
                        EnableRegex = true,
                    };

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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        TargetScopeContains = new[] { "LordOfTheRingsBook" },
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Struct,
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        TargetScopeExcludes = new[] { "LordOfTheRingsBook" },
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Struct,
                        TargetScopeExcludes = new[] { "HarryPotterBook" },
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        TargetNameContains = new[] { "Read" },
                        TargetScopeContains = new[] { "LordOfTheRingsBook" },
                    };

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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Struct,
                        TargetNameContains = new[] { "Read" },
                        TargetScopeContains = new[] { "HarryPotterBook" },
                    };

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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        AccessModifier = QueryAccessModifier.Private,
                        Scope = QueryTargetScope.Class,
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        AccessModifier = QueryAccessModifier.Private,
                        TargetScopeContains = new[] { "Harry" },
                    };

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
                    Assert.Contains("UseCloak", SelectNames(this.results));
                }
            }

            public class GetPublicMethodsInSpecificClass
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetPublicMethodsInSpecificClass()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Method,
                        Scope = QueryTargetScope.Class,
                        AccessModifier = QueryAccessModifier.Public,
                        TargetScopeContains = new[] { "Harry" },
                    };

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
                    IEnumerable<string?> names = SelectNames(this.results);
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Class,
                        AccessModifier = QueryAccessModifier.Public,
                    };

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
                    Assert.Contains("Warrior", SelectNames(this.results));
                }
            }

            public class GetProtectedInternalClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetProtectedInternalClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Class,
                        AccessModifier = QueryAccessModifier.ProtectedInternal,
                    };

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
                    Assert.Contains("Spy", SelectNames(this.results));
                }
            }

            public class GetProtectedClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetProtectedClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Class,
                        AccessModifier = QueryAccessModifier.Protected,
                    };

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
                    Assert.Contains("Spy", SelectNames(this.results));
                }
            }

            public class GetInternalClasses
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetInternalClasses()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.Class,
                        AccessModifier = QueryAccessModifier.Internal,
                    };

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
                    Assert.Contains("PoliceMan", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetProtectedInternalClassName()
                {
                    Assert.Contains("Spy", SelectNames(this.results));
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

                        public string GetPurrText(int purrStrength)
                        {
                            var catName = ""Felix"";
                            Console.WriteLine($""Log: {catName} starts purring."");
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor();

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetAllSyntaxNodesThatHaveName()
                {
                    Assert.Equal(19, this.results.Count);
                }

                [Fact]
                public void ShouldGetNamespace()
                {
                    Assert.Contains("Zoo", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetClass()
                {
                    Assert.Contains("Cat", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetStruct()
                {
                    Assert.Contains("Sun", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetEnum()
                {
                    Assert.Contains("WeatherState", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetInterface()
                {
                    Assert.Contains("IAnimal", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetField()
                {
                    Assert.Contains("lives", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetProperty()
                {
                    Assert.Contains("RemainingLives", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetEvent()
                {
                    Assert.Contains("OnWakeUp", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetMethod()
                {
                    Assert.Contains("GetPurrText", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetVariable()
                {
                    Assert.Contains("catName", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetMethodParameter()
                {
                    Assert.Contains("purrStrength", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetInvocationName()
                {
                    Assert.Contains(
                        "$\"Log: {catName} starts purring.\"",
                        SelectNames(this.results));
                }
            }

            public class GetAnyTargetPublicModifiers
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetAnyTargetPublicModifiers()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        AccessModifier = QueryAccessModifier.Public,
                    };

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetAllSyntaxNodes()
                {
                    Assert.Equal(7, this.results.Count);
                }

                [Fact]
                public void ShouldNotGetNamespace()
                {
                    Assert.DoesNotContain("Zoo", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetClass()
                {
                    Assert.Contains("Cat", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetStruct()
                {
                    Assert.Contains("Sun", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetEnum()
                {
                    Assert.Contains("WeatherState", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetInterface()
                {
                    Assert.Contains("IAnimal", SelectNames(this.results));
                }

                [Fact]
                public void ShouldNotGetField()
                {
                    Assert.DoesNotContain("lives", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetProperty()
                {
                    Assert.Contains("RemainingLives", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetEvent()
                {
                    Assert.Contains("OnWakeUp", SelectNames(this.results));
                }

                [Fact]
                public void ShouldGetMethod()
                {
                    Assert.Contains("GetPurrText", SelectNames(this.results));
                }

                [Fact]
                public void ShouldNotGetVariable()
                {
                    Assert.DoesNotContain("catName", SelectNames(this.results));
                }
            }

            public class GetVariables
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetVariables()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = QueryTarget.LocalVariable,
                    };

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    var walker = new SyntaxTreeQueryWalker(query);
                    walker.Visit(syntaxTree.GetCompilationUnitRoot());
                    this.results = walker.Results;
                }

                [Fact]
                public void ShouldGetAllSyntaxNodes()
                {
                    Assert.Equal(1, this.results.Count);
                }

                [Fact]
                public void ShouldGetVariable()
                {
                    Assert.Contains("catName", SelectNames(this.results));
                }
            }
        }

        public static class MultipleQueryTargets
        {
            private const string SourceCode = @"
                namespace SolarSystem
                {
                    public class Earth
                    {
                        public string Spin()
                        {
                            string action = ""Spinning!"";
                            return action;
                        }

                        public string SpinFaster()
                        {
                            string action = ""Spinning Faster!"";
                            return action;
                        }
                        
                        public string SpinSlower()
                        {
                            string action = ""Spinning Slower!"";
                            return action;
                        }
                    }

                    public class Mars
                    {
                    }
                }
            ";

            public class ContainsAndExclude
            {
                private readonly SyntaxTree tree;

                public ContainsAndExclude()
                {
                    this.tree = CSharpSyntaxTree.ParseText(SourceCode);
                }

                [Fact]
                public void Visit_MethodContainsSpin_ThreeMethods()
                {
                    SyntaxTreeQueryWalker? walker = Init(
                        QueryTarget.Method,
                        new[] { "Spin" });

                    walker.Visit(this.tree.GetCompilationUnitRoot());

                    Assert.Equal(
                        new[] { "Spin", "SpinFaster", "SpinSlower" },
                        SelectNames(walker.Results));
                }

                [Fact]
                public void Visit_MethodContainsFasterAndSlower_TwoMethods()
                {
                    SyntaxTreeQueryWalker? walker = Init(
                        QueryTarget.Method,
                        new[] { "Faster", "Slower" });

                    walker.Visit(this.tree.GetCompilationUnitRoot());

                    Assert.Equal(
                        new[] { "SpinFaster", "SpinSlower" },
                        SelectNames(walker.Results));
                }

                [Fact]
                public void Visit_MethodContainsSpinButNotSlowerAndFaster_SingleMethod()
                {
                    SyntaxTreeQueryWalker? walker = Init(
                        QueryTarget.Method,
                        new[] { "Spin" },
                        new[] { "Slower", "Faster" });

                    walker.Visit(this.tree.GetCompilationUnitRoot());

                    Assert.Equal(
                        new[] { "Spin" },
                        SelectNames(walker.Results));
                }

                [Fact]
                public void Visit_ClassContainsRegexp_TwoClasses()
                {
                    SyntaxTreeQueryWalker? walker = Init(
                        QueryTarget.Class,
                        new[] { @".*ar.*" },
                        enableRegex: true);

                    walker.Visit(this.tree.GetCompilationUnitRoot());

                    Assert.Equal(
                        new[] { "Earth", "Mars" },
                        SelectNames(walker.Results));
                }

                [Fact]
                public void Visit_ClassContainsRegexpExcluding_SingleClass()
                {
                    SyntaxTreeQueryWalker? walker = Init(
                        QueryTarget.Class,
                        new[] { @".*ar.*" },
                        new[] { @"^.{4,4}$" },
                        enableRegex: true);

                    walker.Visit(this.tree.GetCompilationUnitRoot());

                    Assert.Equal(
                        new[] { "Earth" },
                        SelectNames(walker.Results));
                }

                private static SyntaxTreeQueryWalker Init(
                    QueryTarget target,
                    IEnumerable<string>? contains = null,
                    IEnumerable<string>? excludes = null,
                    bool enableRegex = false)
                {

                    var queryDescriptor = new SyntaxTreeQueryDescriptor
                    {
                        Target = target,
#pragma warning disable CS8601 // Possible null reference assignment.
                        TargetNameContains = contains,
                        TargetNameExcludes = excludes,
#pragma warning restore CS8601 // Possible null reference assignment.
                        EnableRegex = enableRegex,
                    };

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    return new SyntaxTreeQueryWalker(query);
                }

            }
        }

        private static string?[] SelectNames(IEnumerable<SyntaxNode> nodes)
        {
            return nodes.Select(x => x.TryGetIdentifierName()).OrderBy(x => x).ToArray();
        }
    }
}
