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
    public class IfConditionVirtualQueryTests
    {
        private readonly IFixture fixture;
        private readonly IfConditionVirtualQuery sut;

        public IfConditionVirtualQueryTests()
        {
            this.fixture = AutoFixtureFactory.Default();
            this.sut = this.fixture.Create<IfConditionVirtualQuery>();
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
        public void CanQuery_IfCondition_True()
        {
            Assert.True(this.sut.CanQuery(CreateIfConditionExpression()));
        }

        [Fact]
        public void CanQuery_NotIfCondition_False()
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
        public void Query_IfCondition_VirtualNode()
        {
            Assert.IsType<IfConditionSyntax>(
                this.sut.Query(CreateIfConditionExpression()));
        }

        [Fact]
        public void Query_NotIfCondition_Throws()
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

        private static ExpressionSyntax CreateIfConditionExpression()
        {
            const string sourceText =
                "public class C1 { void Method() { if (x == 3) {} } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<IfStatementSyntax>().Condition;
        }

        private static ExpressionSyntax CreateNotIfExpression()
        {
            const string sourceText =
                "public class C1 { void Method(int x) { x++; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            return tree.GetRoot().GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                .Expression;
        }
    }
}
