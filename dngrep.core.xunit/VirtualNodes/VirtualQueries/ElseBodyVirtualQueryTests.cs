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
    public class ElseBodyVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly ElseBodyVirtualQuery sut;

        public ElseBodyVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<ElseBodyVirtualQuery>();
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
        public void CanQuery_ElseBody_True()
        {
            Assert.True(this.sut.CanQuery(CreateElseBodyExpression()));
        }

        [Fact]
        public void CanQuery_NotElseBody_False()
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
        public void Query_ElseBody_VirtualNode()
        {
            Assert.IsType<ElseBodySyntax>(
                this.sut.Query(CreateElseBodyExpression()));
        }

        [Fact]
        public void Query_NotElseBody_Throws()
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

        private static BlockSyntax CreateElseBodyExpression()
        {
            const string sourceText =
                "public class C1 { void Method() { if (x == 3) {} else {} } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

#pragma warning disable CS8603 // Possible null reference return.
            return tree.GetRoot().GetFirstChildOfTypeRecursively<IfStatementSyntax>().Else
                ?.Statement as BlockSyntax;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private static BlockSyntax CreateNotIfExpression()
        {
            const string sourceText =
                "public class C1 { void Method() { if (x == 3) {} } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

#pragma warning disable CS8603 // Possible null reference return.
            return tree.GetRoot().GetFirstChildOfTypeRecursively<IfStatementSyntax>().Statement
                as BlockSyntax;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
