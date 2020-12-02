using System;
using System.Linq;
using AutoFixture;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.Queries.SyntaxNodeMatchers;
using dngrep.core.Queries.SyntaxNodeMatchers.Boolean;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using SharpJuice.AutoFixture;
using Xunit;

namespace dngrep.core.xunit.Queries
{
    public static class SyntaxTreeQueryBuilderTests
    {
        public class TargetTypeTests
        {
            private readonly IFixture fixture;

            public TargetTypeTests()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.fixture.Inject(QueryAccessModifier.Any);
                this.fixture.Inject(QueryTargetScope.Class);
            }

            [Fact()]
            public void FromDescriptor_Any_AnyDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Any);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Empty(result.TargetMatchers);
            }

            [Fact]
            public void FromDescriptor_Class_ClassDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Class);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.ClassDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Enum_EnumDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Enum);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.EnumDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Field_FieldDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Field);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.ParseTypeName("bool"),
                            SyntaxFactory.SeparatedList(
                                new[] { SyntaxFactory.VariableDeclarator("any") })))));
            }

            [Fact]
            public void FromDescriptor_Interface_InterfaceDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Interface);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.InterfaceDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Method_MethodDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Method);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(
                        SyntaxFactory.MethodDeclaration(
                            SyntaxFactory.ParseTypeName("bool"),
                            "any")));
            }

            [Fact]
            public void FromDescriptor_MethodArgument_MethodArgumentDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.InvocationArgument);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(
                        SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("any"),
                                SyntaxFactory.IdentifierName("any")))));
            }

            [Fact]
            public void FromDescriptor_Namespace_NamespaceDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Namespace);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(
                        SyntaxFactory.NamespaceDeclaration(
                            SyntaxFactory.ParseName("any"))));
            }

            [Fact]
            public void FromDescriptor_Property_PropertyDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Property);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(
                        SyntaxFactory.PropertyDeclaration(
                            SyntaxFactory.ParseTypeName("bool"),
                            "any")));
            }

            [Fact]
            public void FromDescriptor_Struct_StructDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.Struct);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.StructDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Variable_VariableDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTarget.LocalVariable);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(
                        SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.ParseTypeName("bool"),
                                SyntaxFactory.SeparatedList(
                                    new[] { SyntaxFactory.VariableDeclarator("any") })))));
            }

            private static SyntaxTreeQueryDescriptor DescribeQuery(QueryTarget target)
            {
                return new SyntaxTreeQueryDescriptor
                {
                    Target = target,
                };
            }
        }

        public class AccessModifiersTests
        {
            private readonly IFixture fixture;

            public AccessModifiersTests()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.fixture.Inject(QueryTarget.Class);
                this.fixture.Inject(QueryTargetScope.Class);
            }

            [Fact]
            public void FromDescriptor_AnyModifier_EmptyCollection()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.Any)
                    .Create();

                Assert.Empty(SyntaxTreeQueryBuilder.From(queryDescriptor).AccessModifierMatchers);
            }

            [Fact]
            public void FromDescriptor_PublicModifier_PublicKind()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.Public)
                    .Create();

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(result.AccessModifierMatchers,
                    x => x.Match(InitNodeWithModifiers(SyntaxKind.PublicKeyword)));
            }

            [Fact]
            public void FromDescriptor_PrivateModifier_PrivateKind()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.Private)
                    .Create();

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(
                    result.AccessModifierMatchers,
                    x => x.Match(
                        InitNodeWithModifiers(
                            SyntaxKind.PrivateKeyword)));
            }

            [Fact]
            public void FromDescriptor_ProtectedModifier_ProtectedKind()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.Protected)
                    .Create();

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(
                    result.AccessModifierMatchers,
                    x => x.Match(
                        InitNodeWithModifiers(
                            SyntaxKind.ProtectedKeyword)));
            }

            [Fact]
            public void FromDescriptor_InternalModifier_InternalKind()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.Internal)
                    .Create();

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(
                    result.AccessModifierMatchers,
                    x => x.Match(
                        InitNodeWithModifiers(
                            SyntaxKind.InternalKeyword)));
            }

            [Fact]
            public void FromDescriptor_ProtectedInternalModifier_ProtectedInternalKind()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.ProtectedInternal)
                    .Create();

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(
                    result.AccessModifierMatchers,
                    x => x.Match(
                        InitNodeWithModifiers(
                            SyntaxKind.ProtectedKeyword,
                            SyntaxKind.InternalKeyword)));
            }

            [Fact]
            public void FromDescriptor_PrivateProtectedModifier_PrivateProtectedKind()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.fixture
                    .Build<SyntaxTreeQueryDescriptor>()
                    .With(x => x.AccessModifier, QueryAccessModifier.PrivateProtected)
                    .Create();

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(
                    result.AccessModifierMatchers,
                    x => x.Match(
                        InitNodeWithModifiers(
                            SyntaxKind.PrivateKeyword,
                            SyntaxKind.ProtectedKeyword)));
            }

            private static SyntaxNode InitNodeWithModifiers(params SyntaxKind[] modifiers)
            {
                return SyntaxFactory.ClassDeclaration("any")
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            modifiers.Select(x => SyntaxFactory.Token(x))));
            }
        }

        public class PathTests
        {
            private readonly IFixture fixture;

            public PathTests()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.fixture.Inject(QueryTarget.Class);
                this.fixture.Inject(QueryTargetScope.Class);
                this.fixture.Inject(QueryAccessModifier.Any);
            }

            [Fact]
            public void FromDescriptor_Empty_EmptyCollection()
            {
                var queryDescriptor = new SyntaxTreeQueryDescriptor();

                Assert.Empty(SyntaxTreeQueryBuilder.From(queryDescriptor).PathMatchers);
            }

            [Fact]
            public void FromDescriptor_ContainsPathAndNoRegex_SingleContainsPathMatcher()
            {
                var queryDescriptor = new SyntaxTreeQueryDescriptor
                {
                    TargetPathContains = new[] { "SuperFolder" }
                };

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.PathMatchers);
                Assert.Contains(
                    result.PathMatchers,
                    x => x.GetType() == typeof(ContainsPathMatcher));
            }

            [Fact]
            public void FromDescriptor_ContainsMultiplePathAndNoRegex_SingleContainsPathMatcher()
            {
                var queryDescriptor = new SyntaxTreeQueryDescriptor
                {
                    TargetPathContains = new[] { "SuperFolder1", "SuperFolder2" }
                };

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(2, result.PathMatchers.Count);
                Assert.True(
                    result.PathMatchers.All(
                        x => x.GetType() == typeof(ContainsPathMatcher)));
            }

            [Fact]
            public void FromDescriptor_ContainsPathAndRegex_SingleContainsRegexPathMatcher()
            {
                var queryDescriptor = new SyntaxTreeQueryDescriptor
                {
                    TargetPathContains = new[] { "SuperFolder" },
                    EnableRegex = true
                };

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.PathMatchers);
                Assert.Contains(
                    result.PathMatchers,
                    x => x.GetType() == typeof(ContainsPathRegexMatcher));
            }

            [Fact]
            public void FromDescriptor_ContainsMultiplePathAndRegex_SingleContainsPathMatcher()
            {
                var queryDescriptor = new SyntaxTreeQueryDescriptor
                {
                    TargetPathContains = new[] { "SuperFolder1", "SuperFolder2" },
                    EnableRegex = true
                };

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(2, result.PathMatchers.Count);
                Assert.True(
                    result.PathMatchers.All(
                        x => x.GetType() == typeof(ContainsPathRegexMatcher)));
            }
        }

        public class ScopeTests
        {
            private readonly IFixture fixture;

            public ScopeTests()
            {
                this.fixture = AutoFixtureFactory.Default();
                this.fixture.Inject(QueryTarget.Class);
                this.fixture.Inject(QueryAccessModifier.Any);
            }

            [Fact]
            public void FromDescriptor_NoneScope_Null()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTargetScope.None);

                Assert.Empty(SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeMatchers);
            }

            [Fact]
            public void FromDescriptor_ClassScope_ClassDeclarationSyntax()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTargetScope.Class);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(SyntaxFactory.ClassDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_StructScope_StructDeclarationSyntax()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = DescribeQuery(QueryTargetScope.Struct);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(SyntaxFactory.StructDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_NamespaceScope_NamespaceDeclarationSyntax()
            {
                SyntaxTreeQueryDescriptor queryDescriptor =
                    DescribeQuery(QueryTargetScope.Namespace);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(
                        SyntaxFactory.NamespaceDeclaration(
                            SyntaxFactory.ParseName("any"))));
            }

            [Fact]
            public void FromDescriptor_InterfaceScope_InterfaceDeclarationSyntax()
            {
                SyntaxTreeQueryDescriptor queryDescriptor =
                    DescribeQuery(QueryTargetScope.Interface);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(SyntaxFactory.InterfaceDeclaration("any")));
            }

            private static SyntaxTreeQueryDescriptor DescribeQuery(
                QueryTargetScope scope,
                bool enableRegex = false)
            {
                return new SyntaxTreeQueryDescriptor
                {
                    Scope = scope,
                    EnableRegex = enableRegex
                };
            }
        }

        public class FromTextSpanTests
        {
            [Fact]
            public void FromTextSpan_Null_Throws()
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                Assert.Throws<ArgumentNullException>(() => SyntaxTreeQueryBuilder.From(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            [Fact]
            public void FromTextSpan_NotNull_SingleTargetMatcher()
            {
                Assert.Single(SyntaxTreeQueryBuilder.From(new TextSpan(0, 1)).TargetMatchers);
            }

            [Fact]
            public void FromTextSpan_NotNull_AndMatcher()
            {
                Assert.Equal(
                    typeof(And),
                    SyntaxTreeQueryBuilder.From(new TextSpan(0, 1))
                        .TargetMatchers
                        .First()
                        .GetType());
            }

            [Fact]
            public void FromTextSpan_NotNull_AndContainsKnownTargetMatcher()
            {
                Assert.Equal(
                    typeof(KnownTargetMatcher),
                    (SyntaxTreeQueryBuilder.From(new TextSpan(0, 1))
                        .TargetMatchers
                        .First() as And)?.Lhs.GetType()
                        );
            }

            [Fact]
            public void FromTextSpan_NotNull_AndContainsSourceTextMatcher()
            {
                Assert.Equal(
                    typeof(SourceTextPositionMatcher),
                    (SyntaxTreeQueryBuilder.From(new TextSpan(0, 1))
                        .TargetMatchers
                        .First() as And)?.Rhs.GetType());
            }

            [Fact]
            public void FromTextSpan_NotNull_AndContainsSourceTextMatcherWithCorrectSpan()
            {
                var span = new TextSpan(0, 1);

                Assert.Equal(
                    span,
                    ((SyntaxTreeQueryBuilder.From(span)
                        .TargetMatchers
                        .First() as And)?.Rhs as SourceTextPositionMatcher)?.SpanToMatch );
            }
        }
    }
}
