using System.Linq;
using AutoFixture;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Any);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Empty(result.TargetMatchers);
            }

            [Fact]
            public void FromDescriptor_Class_ClassDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Class);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.ClassDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Enum_EnumDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Enum);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.EnumDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Field_FieldDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Field);

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
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Interface);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.InterfaceDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Method_MethodDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Method);

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
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.InvocationArgument);

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
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Namespace);

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
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Property);

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
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Struct);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.TargetMatchers,
                    x => x.Match(SyntaxFactory.StructDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_Variable_VariableDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.LocalVariable);

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

            private SyntaxTreeQueryDescriptor DescribeQuery(QueryTarget target)
            {
                return this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        queryTarget = target,
                        targetScope = QueryTargetScope.None,
                        accessModifier = QueryAccessModifier.Any,
                        targetNameContains = Enumerable.Empty<string>(),
                        targetNameExcludes = Enumerable.Empty<string>(),
                        scopeContains = Enumerable.Empty<string>(),
                        scopeExclude = Enumerable.Empty<string>(),
                        enableRegex = false
                    });
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
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.Any
                    });

                Assert.Empty(SyntaxTreeQueryBuilder.From(queryDescriptor).AccessModifierMatchers);
            }

            [Fact]
            public void FromDescriptor_PublicModifier_PublicKind()
            {
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.Public
                    });

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Single(result.AccessModifierMatchers);
                Assert.Contains(result.AccessModifierMatchers,
                    x => x.Match(InitNodeWithModifiers(SyntaxKind.PublicKeyword)));
            }

            [Fact]
            public void FromDescriptor_PrivateModifier_PrivateKind()
            {
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.Private
                    });

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
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.Protected
                    });

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
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.Internal
                    });

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
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.ProtectedInternal
                    });

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
                var queryDescriptor = this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        AccessModifier = QueryAccessModifier.PrivateProtected
                    });

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
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.None);

                Assert.Empty(SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeMatchers);
            }

            [Fact]
            public void FromDescriptor_ClassScope_ClassDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Class);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(SyntaxFactory.ClassDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_StructScope_StructDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Struct);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(SyntaxFactory.StructDeclaration("any")));
            }

            [Fact]
            public void FromDescriptor_NamespaceScope_NamespaceDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Namespace);

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
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Interface);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Contains(
                    result.ScopeMatchers,
                    x => x.Match(SyntaxFactory.InterfaceDeclaration("any")));
            }

            private SyntaxTreeQueryDescriptor DescribeQuery(
                QueryTargetScope scope,
                bool enableRegex = false)
            {
                return this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new
                    {
                        targetScope = scope,
                        accessModifier = QueryAccessModifier.Any,
                        targetNameContains = Enumerable.Empty<string>(),
                        targetNameExcludes = Enumerable.Empty<string>(),
                        scopeContains = Enumerable.Empty<string>(),
                        scopeExclude = Enumerable.Empty<string>(),
                        enableRegex
                    });
            }
        }
    }
}
