using AutoFixture;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

                Assert.Null(result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Class_ClassDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Class);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(ClassDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Enum_EnumDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Enum);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(EnumDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Field_FieldDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Field);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(FieldDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Interface_InterfaceDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Interface);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(InterfaceDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Method_MethodDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Method);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(MethodDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_MethodArgument_MethodArgumentDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Method);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(MethodDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Namespace_NamespaceDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Namespace);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(NamespaceDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Property_PropertyDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Property);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(PropertyDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Struct_StructDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.Struct);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(StructDeclarationSyntax), result.TargetType);
            }

            [Fact]
            public void FromDescriptor_Variable_VariableDeclaration()
            {
                SyntaxTreeQueryDescriptor queryDescriptor = this.DescribeQuery(QueryTarget.LocalVariable);

                SyntaxTreeQuery result = SyntaxTreeQueryBuilder.From(queryDescriptor);

                Assert.Equal(typeof(LocalDeclarationStatementSyntax), result.TargetType);
            }

            private SyntaxTreeQueryDescriptor DescribeQuery(QueryTarget target)
            {
                return this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new { queryTarget = target });
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

                Assert.Empty(SyntaxTreeQueryBuilder.From(queryDescriptor).TargetAccessModifiers);
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

                Assert.Single(result.TargetAccessModifiers);
                Assert.Contains(SyntaxKind.PublicKeyword, result.TargetAccessModifiers);
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

                Assert.Single(result.TargetAccessModifiers);
                Assert.Contains(SyntaxKind.PrivateKeyword, result.TargetAccessModifiers);
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

                Assert.Single(result.TargetAccessModifiers);
                Assert.Contains(SyntaxKind.ProtectedKeyword, result.TargetAccessModifiers);
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

                Assert.Single(result.TargetAccessModifiers);
                Assert.Contains(SyntaxKind.InternalKeyword, result.TargetAccessModifiers);
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

                Assert.Equal(2, result.TargetAccessModifiers.Count);
                Assert.Contains(SyntaxKind.ProtectedKeyword, result.TargetAccessModifiers);
                Assert.Contains(SyntaxKind.InternalKeyword, result.TargetAccessModifiers);
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

                Assert.Equal(2, result.TargetAccessModifiers.Count);
                Assert.Contains(SyntaxKind.PrivateKeyword, result.TargetAccessModifiers);
                Assert.Contains(SyntaxKind.ProtectedKeyword, result.TargetAccessModifiers);
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

                Assert.Null(SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeType);
            }

            [Fact]
            public void FromDescriptor_ClassScope_ClassDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Class);

                Assert.Equal(
                    typeof(ClassDeclarationSyntax),
                    SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeType);
            }

            [Fact]
            public void FromDescriptor_StructScope_StructDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Struct);

                Assert.Equal(
                    typeof(StructDeclarationSyntax),
                    SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeType);
            }

            [Fact]
            public void FromDescriptor_NamespaceScope_NamespaceDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Namespace);

                Assert.Equal(
                    typeof(NamespaceDeclarationSyntax),
                    SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeType);
            }

            [Fact]
            public void FromDescriptor_InterfaceScope_InterfaceDeclarationSyntax()
            {
                var queryDescriptor = this.DescribeQuery(QueryTargetScope.Interface);

                Assert.Equal(
                    typeof(InterfaceDeclarationSyntax),
                    SyntaxTreeQueryBuilder.From(queryDescriptor).ScopeType);
            }

            private SyntaxTreeQueryDescriptor DescribeQuery(QueryTargetScope scope)
            {
                return this.fixture.Create<SyntaxTreeQueryDescriptor>(
                    new { targetScope = scope });
            }
        }
    }
}
