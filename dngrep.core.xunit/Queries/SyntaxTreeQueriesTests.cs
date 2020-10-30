﻿using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
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
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Method,
                        QueryAccessModifier.Any,
                        QueryTargetScope.Class,
                        new[] { "Read" },
                        null,
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
                        new[] { "Read" },
                        null,
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
                        new[] { "Read" },
                        null,
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

            public class GetAnyTargetPublicModifiers
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetAnyTargetPublicModifiers()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.Any,
                        QueryAccessModifier.Public,
                        QueryTargetScope.None,
                        null,
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
                    Assert.Equal(7, this.results.Count);
                }

                [Fact]
                public void ShouldNotGetNamespace()
                {
                    Assert.DoesNotContain("Zoo", this.results.Select(x => x.TryGetIdentifierName()));
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
                public void ShouldNotGetField()
                {
                    Assert.DoesNotContain("lives", this.results.Select(x => x.TryGetIdentifierName()));
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
                public void ShouldNotGetVariable()
                {
                    Assert.DoesNotContain("catName", this.results.Select(x => x.TryGetIdentifierName()));
                }
            }

            public class GetVariables
            {
                private readonly IReadOnlyCollection<SyntaxNode> results;

                public GetVariables()
                {
                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        QueryTarget.LocalVariable,
                        QueryAccessModifier.Any,
                        QueryTargetScope.None,
                        null,
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
                    Assert.Equal(1, this.results.Count);
                }

                [Fact]
                public void ShouldGetVariable()
                {
                    Assert.Contains("catName", this.results.Select(x => x.TryGetIdentifierName()));
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
                        walker.Results.Select(x => x.TryGetIdentifierName()));
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
                        walker.Results.Select(x => x.TryGetIdentifierName()));
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
                        walker.Results.Select(x => x.TryGetIdentifierName()));
                }

                private static SyntaxTreeQueryWalker Init(
                    QueryTarget target,
                    IEnumerable<string>? contains = null,
                    IEnumerable<string>? excludes = null)
                {

                    var queryDescriptor = new SyntaxTreeQueryDescriptor(
                        target,
                        QueryAccessModifier.Any,
                        QueryTargetScope.None,
                        contains,
                        excludes,
                        null
                        );

                    SyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(queryDescriptor);

                    return new SyntaxTreeQueryWalker(query);
                }
            }
        }
    }
}
