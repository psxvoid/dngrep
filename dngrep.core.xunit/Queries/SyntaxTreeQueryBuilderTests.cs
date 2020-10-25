using AutoFixture;
using dngrep.core.Queries;
using dngrep.core.Queries.Specifiers;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis.CSharp;
using SharpJuice.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace dngrep.core.xunit.Queries
{
    public static class SyntaxTreeQueryBuilderTests
    {
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
    }
}
