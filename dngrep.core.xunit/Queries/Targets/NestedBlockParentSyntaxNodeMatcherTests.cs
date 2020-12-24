using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.Queries.Targets
{
    public class NestedBlockParentSyntaxNodeMatcherTests
    {
        [Fact]
        public void Match_ClassBlock_False()
        {
            const string target =
                "public class C { public int M() { }";

            AssertMatch<ClassDeclarationSyntax>(
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
            AssertMatch<ConstructorDeclarationSyntax>(target, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_ConstructorExpressionBody_False()
        {
            const string target =
                "using System; public class C { C() => Console.WriteLine(5); }";

#pragma warning disable CS8603 // Possible null reference return.
            AssertMatch<ConstructorDeclarationSyntax>(target, false, x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_DestructorBody_False()
        {
            const string target =
                "public class C { ~C() { } }";

#pragma warning disable CS8603 // Possible null reference return.
            AssertMatch<DestructorDeclarationSyntax>(target, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_DestructorExpressionBody_False()
        {
            const string target =
                "using System; public class C { ~C() => Console.WriteLine(5); }";

#pragma warning disable CS8603 // Possible null reference return.
            AssertMatch<DestructorDeclarationSyntax>(target, false, x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_MethodBody_False()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

#pragma warning disable CS8603 // Possible null reference return.
            AssertMatch<MethodDeclarationSyntax>(target, false, x => x.Body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_MethodExpressionBody_False()
        {
            const string target =
                "using System; public class C { public int M() => Console.WriteLine(5); }";

#pragma warning disable CS8603 // Possible null reference return.
            AssertMatch<MethodDeclarationSyntax>(target, false, x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_TryBlock_True()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

            AssertMatch<TryStatementSyntax>(target, true, x => x.Block);
        }

        [Fact]
        public void Match_FinallyBlock_True()
        {
            const string target =
                "public class C { public int M() { try { return 5; } finally {} } }";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            AssertMatch<TryStatementSyntax>(target, true, x => x.Finally.Block);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Fact]
        public void Match_IfBlockStatement_False()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } } }";

            AssertMatch<IfStatementSyntax>(
                target,
                false,
                x => x.Statement);
        }

        [Fact]
        public void Match_IfBlockElseStatement_True()
        {
            const string target =
                "public class C { public int M() { if (true) { return 5; } else { return 3; } } }";

            AssertMatch<IfStatementSyntax>(
                target,
                true,
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                x => x.Else.Statement);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Fact]
        public void Match_LocalFunctionBody_False()
        {
            const string target =
                "public class C { public int M() { int X() { return 5; }; return X(); } }";

            AssertMatch<LocalFunctionStatementSyntax>(
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

            AssertMatch<LocalFunctionStatementSyntax>(
                target,
                false,
#pragma warning disable CS8603 // Possible null reference return.
                x => x.ExpressionBody);
#pragma warning restore CS8603 // Possible null reference return.
        }

        [Fact]
        public void Match_ForStatement_False()
        {
            const string target =
                "public class C { public int M() { for(int i=0; i<2; i++) { } } }";

            AssertMatch<ForStatementSyntax>(
                target,
                false,
                x => x.Statement);
        }

        [Fact]
        public void Match_WhileStatement_False()
        {
            const string target =
                "public class C { public int M() { while(true) { } } }";

            AssertMatch<WhileStatementSyntax>(
                target,
                false,
                x => x.Statement);
        }

        [Fact]
        public void Match_SwitchStatementEmptySections_False()
        {
            const string target =
                "public class C { public int M(int x) { switch(x) {   }; return x; } }";

            AssertMatch<SwitchStatementSyntax>(target, false, x => x);
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

            AssertMatch<SwitchStatementSyntax>(
                target,
                false,
                x => x.Sections.First());
        }

        private static void AssertMatch<TNode>(
            string targetCode,
            bool expected,
            Func<TNode, SyntaxNode> selectNode)
            where TNode : SyntaxNode
        {
            SyntaxTree? tree = CSharpSyntaxTree.ParseText(targetCode);

            TNode root = tree.GetRoot().GetFirstChildOfTypeRecursively<TNode>();

            SyntaxNode target = selectNode(root);

            Assert.Equal(expected, NestedBlockParentSyntaxNodeMatcher.Instance.Match(target));
        }
    }
}
