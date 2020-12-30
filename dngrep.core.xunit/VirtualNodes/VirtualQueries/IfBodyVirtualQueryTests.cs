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
    public class IfBodyVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly IfBodyVirtualQuery sut;

        public IfBodyVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<IfBodyVirtualQuery>();
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
        public void CanQuery_IfBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateIfBodyExpression()));
        }

        [Fact]
        public void CanQuery_NotIfBody_False()
        {
            Assert.False(this.sut.CanQuery(CreateNotIfExpression()));
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
        public void Query_IfBody_VirtualNode()
        {
            Assert.IsType<IfBodySyntax>(
                this.sut.Query(CreateIfBodyExpression()));
        }

        [Fact]
        public void Query_NotIfBody_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => this.sut.Query(CreateNotIfExpression()));
        }

        [Fact]
        public void Query_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => this.sut.Query(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        private static StatementSyntax CreateIfBodyExpression()
        {
            const string sourceText =
                "public class C1 { void Method() { if (x == 3) {} } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<IfStatementSyntax>().Statement;
        }

        private static StatementSyntax CreateNotIfExpression()
        {
            const string sourceText =
                "public class C1 { void Method(int x) { x++; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

#pragma warning disable CS8603 // Possible null reference return.
            return tree.GetRoot().GetFirstChildOfTypeRecursively<MethodDeclarationSyntax>().Body;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
