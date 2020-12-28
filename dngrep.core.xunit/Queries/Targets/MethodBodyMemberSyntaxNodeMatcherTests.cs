using System.Linq;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.xunit.Queries.Targets.BaseTests;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.Queries.Targets
{
    public class MethodBodyMemberSyntaxNodeMatcherTests : SyntaxNodeMatcherTestBase
    {
        protected override ISyntaxNodeMatcher Sut => MethodBodyMemberSyntaxNodeMatcher.Instance;

        [Fact]
        public void Match_IfStatement_True()
        {
            const string sourceText =
                "class C { void M() { if (true) {} } }";

            this.AssertMatch<IfStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_TryStatement_True()
        {
            const string sourceText =
                "class C { void M() { try {} finally {} } }";

            this.AssertMatch<TryStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_LocalFunctionStatement_True()
        {
            const string sourceText =
                "class C { void M() { int func() { return 5; } } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_FinallyStatement_False()
        {
            const string sourceText =
                "class C { void M() { try {} finally {} } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<TryStatementSyntax>(sourceText, false, x => x.Finally);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_CatchClause_False()
        {
            const string sourceText =
                "class C { void M() { try {} catch(Exception e) {} } }";

            this.AssertMatch<TryStatementSyntax>(sourceText, false, x => x.Catches.Single());
        }

        [Fact]
        public void Match_LocalDeclarationStatement_True()
        {
            const string sourceText =
                "class C { void M() { var x = 3; } }";

            this.AssertMatch<LocalDeclarationStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_ReturnStatement_True()
        {
            const string sourceText =
                "class C { int M() { return 3; } }";

            this.AssertMatch<ReturnStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_ThrowStatement_True()
        {
            const string sourceText =
                "class C { int M() { throw new Exception(); } }";

            this.AssertMatch<ThrowStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_ExpressionStatement_True()
        {
            const string sourceText =
                "class C { void M(ref int i) { i++; } }";

            this.AssertMatch<ExpressionStatementSyntax>(sourceText, true);
        }

        [Fact]
        public void Match_BlockSyntaxOfMethodBody_False()
        {
            const string sourceText =
                "class C { void M() { {} } }";

            this.AssertMatch<BlockSyntax>(
                sourceText,
                true,
                x => x.ChildNodes().Single(c => c is BlockSyntax));
        }

        [Fact]
        public void Match_BlockSyntaxOfMethodDeclaration_False()
        {
            const string sourceText =
                "class C { void M() { } }";

            this.AssertMatch<BlockSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_BlockSyntaxOfIfStatement_False()
        {
            const string sourceText =
                "class C { void M() { if (true) {} } }";

            this.AssertMatch<IfStatementSyntax>(sourceText, false, x => x.Statement);
        }

        [Fact]
        public void Match_BlockSyntaxOfElseClause_False()
        {
            const string sourceText =
                "class C { void M() { if (true) {} else {} } }";

            this.AssertMatch<ElseClauseSyntax>(sourceText, false, x => x.Statement);
        }

        [Fact]
        public void Match_BlockSyntaxOfTryStatement_False()
        {
            const string sourceText =
                "class C { void M() { try {} finally {} } }";

            this.AssertMatch<TryStatementSyntax>(sourceText, false, x => x.Block);
        }

        [Fact]
        public void Match_BlockSyntaxOfFinallyClause_False()
        {
            const string sourceText =
                "class C { void M() { try {} finally {} } }";

            this.AssertMatch<FinallyClauseSyntax>(sourceText, false, x => x.Block);
        }

        [Fact]
        public void Match_BlockSyntaxOfCatchClause_False()
        {
            const string sourceText =
                "class C { void M() { try {} catch(Exception e) {} } }";

            this.AssertMatch<CatchClauseSyntax>(sourceText, false, x => x.Block);
        }

        [Fact]
        public void Match_BlockSyntaxOfForStatement_False()
        {
            const string sourceText =
                "class C { void M() { for (int i = 0; i < 1; i++) {} } }";

            this.AssertMatch<ForStatementSyntax>(sourceText, false, x => x.Statement);
        }

        [Fact]
        public void Match_BlockSyntaxOfWhileStatement_False()
        {
            const string sourceText =
                "class C { void M() { while(false) {} } }";

            this.AssertMatch<WhileStatementSyntax>(sourceText, false, x => x.Statement);
        }

        [Fact]
        public void Match_BlockSyntaxOfForEachStatement_False()
        {
            const string sourceText =
                "class C { void M() { foreach (var s in string.Empty) {} } }";

            this.AssertMatch<ForEachStatementSyntax>(sourceText, false, x => x.Statement);
        }

        [Fact]
        public void Match_BlockSyntaxOfForLocalFunctionStatement_False()
        {
            const string sourceText =
                "class C { void M() { int func() { return 5; } } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<LocalFunctionStatementSyntax>(sourceText, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
