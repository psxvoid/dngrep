using System;
using AutoFixture;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.VirtualNodes.VirtualQueries
{
    public class TryBodyVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly TryBodyVirtualQuery sut;

        public TryBodyVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<TryBodyVirtualQuery>();
        }

        [Fact]
        public void HasOverride_True()
        {
            Assert.True(this.sut.HasOverride);
        }

        [Fact]
        public void CanOverride_MethodBodyVirtualQuery_True()
        {
            Assert.True(typeof(ICanOverride<MethodBodyVirtualQuery>).IsInstanceOfType(this.sut));
        }

        [Fact]
        public void CanQuery_TryBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateEmptyTryBody()));
        }

        [Fact]
        public void CanQuery_NotTryBody_False()
        {
            Assert.False(this.sut.CanQuery(CreateNonTryBody()));
        }

        [Fact]
        public void CanQuery_Null_False()
        {
            Assert.False(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                this.sut.CanQuery(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Query_TryBody_VirtualNode()
        {
            Assert.IsType<TryBodySyntax>(
                this.sut.Query(CreateEmptyTryBody()));
        }

        [Fact]
        public void Query_NotTryBody_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateNonTryBody()));
        }

        [Fact]
        public void Query_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => this.sut.Query(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        private static SyntaxNode CreateEmptyTryBody()
        {
            const string sourceText =
                "public class C1 { void Method() { try { } finally { } } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<TryStatementSyntax>().Block;
        }

        private static SyntaxNode CreateNonTryBody()
        {
            const string sourceText =
                "public class C1 { void Method() { { } } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot()
                .GetFirstChildOfTypeRecursively<BlockSyntax>()
                .GetFirstChildOfTypeRecursively<BlockSyntax>();
        }
    }
}
