using dngrep.core.Extensions.Nullable;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.xunit.Queries.Targets.BaseTests;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.Queries.Targets
{
    public class NestedBlockParentSyntaxNodeMatcherTests : SyntaxNodeMatcherTestBase
    {
        protected override ISyntaxNodeMatcher Sut => NestedBlockParentSyntaxNodeMatcher.Instance;

        [Fact]
        public void Match_BlockInsideBlock_True()
        {
            const string target = "public class C { void M() { { { } } } }";

            this.AssertMatch<MethodDeclarationSyntax>(
                target,
                true,
#pragma warning disable CS8604 // Possible null reference argument.
                x => x.Body
#pragma warning restore CS8604 // Possible null reference argument.
                    .GetFirstChildOfTypeRecursively<BlockSyntax>()
                    .GetFirstChildOfTypeRecursively<BlockSyntax>());
        }

        [Fact]
        public void Match_ClassDeclaration_False()
        {
            const string target = "public class C { }";

            this.AssertMatch<ClassDeclarationSyntax>(target, false);
        }

        [Fact]
        public void Match_ConstructorBody_True()
        {
            const string target = "public class C { C() { } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<ConstructorDeclarationSyntax>(target, true, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_ConstructorExpressionBody_True()
        {
            const string target =
                "using System; public class C { C() => Console.WriteLine(5); }";

            this.AssertMatch<ConstructorDeclarationSyntax>(
                target,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_DestructorBody_True()
        {
            const string target =
                "public class C { ~C() { } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<DestructorDeclarationSyntax>(target, true, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_DestructorExpressionBody_True()
        {
            const string target =
                "using System; public class C { ~C() => Console.WriteLine(5); }";

            this.AssertMatch<DestructorDeclarationSyntax>(
                target,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_MethodBody_True()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<MethodDeclarationSyntax>(target, true, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_MethodExpressionBody_True()
        {
            const string target =
                "using System; public class C { public int M() => Console.WriteLine(5); }";

            this.AssertMatch<MethodDeclarationSyntax>(
                target,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_TryStatement_True()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

            this.AssertMatch<TryStatementSyntax>(target, true, x => x);
        }

        [Fact]
        public void Match_FinallyStatement_True()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<TryStatementSyntax>(target, true, x => x.Finally);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_CatchStatement_True()
        {
            const string target =
                "public class C { public int M() { try { return 5; } catch(e) {} } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<TryStatementSyntax>(target, true, x => x.Catches.FirstOrDefault());
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_IfStatement_True()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } } }";

            this.AssertMatch<IfStatementSyntax>(target, true);
        }

        [Fact]
        public void Match_IfStatementBlockBody_True()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } } }";

            this.AssertMatch<IfStatementSyntax>(
                target,
                true,
                x => x.Statement.As<BlockSyntax>());
        }

        [Fact]
        public void Match_IfStatementCondition_True()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } } }";

            this.AssertMatch<IfStatementSyntax>(
                target,
                true,
                x => x.Condition);
        }

        [Fact]
        public void Match_IfBlockElseStatement_True()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } else { return 3; } } }";

            this.AssertMatch<IfStatementSyntax>(
                target,
                true,
#pragma warning disable CS8603 // Possible null reference return.
                x => x.Else);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_LocalFunction_False()
        {
            const string target =
                "public class C { public int M() { int X() { return 5; }; return X(); } }";

            // should be MethodBody, not nested block
            this.AssertMatch<LocalFunctionStatementSyntax>(target, false);
        }

        [Fact]
        public void Match_LocalFunctionBody_True()
        {
            const string target =
                "public class C { public int M() { int X() { return 5; }; return X(); } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(
                target,
                true,
#pragma warning disable CS8603 // Possible null reference return.
                x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_LocalFunctionExpressionBody_True()
        {
            const string target =
                "public class C { public int M() { int X() => 5; return X(); } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(
                target,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_ForStatement_True()
        {
            const string target =
                "public class C { public int M() { for(int i=0; i<2; i++) { } } }";

            this.AssertMatch<ForStatementSyntax>(target, true);
        }

        [Fact]
        public void Match_WhileStatement_True()
        {
            const string target =
                "public class C { public int M() { while(true) { } } }";

            this.AssertMatch<WhileStatementSyntax>(target, true);
        }

        [Fact]
        public void Match_SwitchStatementEmptySections_True()
        {
            const string target =
                "public class C { public int M(int x) { switch(x) {   }; return x; } }";

            this.AssertMatch<SwitchStatementSyntax>(target, true);
        }

        [Fact]
        public void Match_AnyStatement_True()
        {
            const string target =
                "public class C { public int M(int x) { lock(this) {   }; return x; } }";

            this.AssertMatch<LockStatementSyntax>(target, true);
        }

        [Fact]
        public void Match_PropertyAccessor_True()
        {
            const string target =
                "public class C { int a; int A { get { return a; } set { a = value; } }";

            this.AssertMatch<AccessorDeclarationSyntax>(target, true);
        }

        [Fact]
        public void Match_SwitchStatementSingleSection_False()
        {
            const string target =
            @"public class C1 {
                public int M(int x)
                {
                    switch(x) {  case 1: return 2; };
                    return x;
                }
            }";

            this.AssertMatch<SwitchStatementSyntax>(
                target,
                false,
                x => x.Sections.First());
        }
    }
}
