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
        public void Match_ClassBlock_False()
        {
            const string target =
                "public class C { public int M() { }";

            this.AssertMatch<ClassDeclarationSyntax>(
                target,
                false,
                x => x.GetFirstChildOfTypeRecursively<BlockSyntax>());
        }

        [Fact]
        public void Match_ConstructorBody_False()
        {
            const string target =
                "public class C { C() { } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<ConstructorDeclarationSyntax>(target, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_ConstructorExpressionBody_False()
        {
            const string target =
                "using System; public class C { C() => Console.WriteLine(5); }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<ConstructorDeclarationSyntax>(target, false, x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_DestructorBody_False()
        {
            const string target =
                "public class C { ~C() { } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<DestructorDeclarationSyntax>(target, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_DestructorExpressionBody_False()
        {
            const string target =
                "using System; public class C { ~C() => Console.WriteLine(5); }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<DestructorDeclarationSyntax>(target, false, x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_MethodBody_False()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<MethodDeclarationSyntax>(target, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_MethodExpressionBody_False()
        {
            const string target =
                "using System; public class C { public int M() => Console.WriteLine(5); }";

#pragma warning disable CS8603 // Possible null reference return.
            this.AssertMatch<MethodDeclarationSyntax>(target, false, x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
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
        public void Match_IfBlockStatement_True()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } } }";

            this.AssertMatch<IfStatementSyntax>(
                target,
                false,
                x => x.Statement);
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
        public void Match_LocalFunctionBody_False()
        {
            const string target =
                "public class C { public int M() { int X() { return 5; }; return X(); } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(
                target,
                false,
#pragma warning disable CS8603 // Possible null reference return.
                x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_LocalFunctionExpressionBody_False()
        {
            const string target =
                "public class C { public int M() { int X() => 5; return X(); } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(
                target,
                false,
#pragma warning disable CS8603 // Possible null reference return.
                x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
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
        public void Match_SwitchStatementEmptySections_False()
        {
            const string target =
                "public class C { public int M(int x) { switch(x) {   }; return x; } }";

            this.AssertMatch<SwitchStatementSyntax>(target, false);
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
