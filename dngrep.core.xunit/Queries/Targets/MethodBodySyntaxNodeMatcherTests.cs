using System.Linq;
using dngrep.core.Extensions.Nullable;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.xunit.Queries.Targets.BaseTests;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace dngrep.core.xunit.Queries.Targets
{
    public class MethodBodySyntaxNodeMatcherTests : SyntaxNodeMatcherTestBase
    {
        protected override ISyntaxNodeMatcher Sut => MethodBodySyntaxNodeMatcher.Instance;

        [Fact]
        public void Match_ClassDeclaration_False()
        {
            const string sourceText = "class C { }";

            this.AssertMatch<ClassDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_MethodDeclaration_False()
        {
            const string sourceText = "class C { int GetX() { return 0; } }";

            this.AssertMatch<MethodDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_MethodDeclarationBlockBody_True()
        {
            const string sourceText = "class C { int GetX() { return 0; } }";

            this.AssertMatch<MethodDeclarationSyntax>(sourceText, true, x => x.Body.NotNull());
        }

        [Fact]
        public void Match_MethodDeclarationExpressionBody_True()
        {
            const string sourceText = "class C { int GetX() => 0; }";

            this.AssertMatch<MethodDeclarationSyntax>(
                sourceText,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_ConstructorDeclaration_False()
        {
            const string sourceText = "class C { C() { } }";

            this.AssertMatch<ConstructorDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_ConstructorDeclarationBlockBody_True()
        {
            const string sourceText = "class C { C() { } }";

            this.AssertMatch<ConstructorDeclarationSyntax>(sourceText, true, x => x.Body.NotNull());
        }

        [Fact]
        public void Match_ConstructorDeclarationExpressionBody_True()
        {
            const string sourceText = "class C { C() => System.Write(5); }";

            this.AssertMatch<ConstructorDeclarationSyntax>(
                sourceText,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_DestructorDeclaration_False()
        {
            const string sourceText = "class C { ~C() { } }";

            this.AssertMatch<DestructorDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_DestructorDeclarationBlockBody_True()
        {
            const string sourceText = "class C { ~C() { } }";

            this.AssertMatch<DestructorDeclarationSyntax>(sourceText, true, x => x.Body.NotNull());
        }

        [Fact]
        public void Match_DestructorDeclarationExpressionBody_True()
        {
            const string sourceText = "class C { ~C() => System.Write(5); }";

            this.AssertMatch<DestructorDeclarationSyntax>(
                sourceText,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_LocalFunction_False()
        {
            const string sourceText = "class C { void M() { int F() { return 0; } } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_LocalFunctionBlockBody_True()
        {
            const string sourceText = "class C { void M() { int F() { return 0; } } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(
                sourceText,
                true,
                x => x.Body.NotNull());
        }

        [Fact]
        public void Match_LocalFunctionExpressionBody_True()
        {
            const string sourceText = "class C { void M() { int F() => 0; } }";

            this.AssertMatch<LocalFunctionStatementSyntax>(
                sourceText,
                true,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_IfCondition_False()
        {
            const string sourceText = "class C { void M() { if(3 == 3) {} } }";

            this.AssertMatch<IfStatementSyntax>(sourceText, false, x => x.Condition);
        }

        [Fact]
        public void Match_IfConditionNestedExpression_False()
        {
            const string sourceText = "class C { void M() { if(3 == 3 && 2 == 2) {} } }";

            this.AssertMatch<IfStatementSyntax>(
                sourceText,
                false,
                x => x.Condition.ChildNodes().OfType<ExpressionSyntax>().First());
        }

        [Fact]
        public void Match_IfBodyStatement_False()
        {
            const string sourceText = "class C { void M(int x) { if(3 == 3) { x++; } } }";

            this.AssertMatch<IfStatementSyntax>(
                sourceText,
                false,
                x => x.Statement.ChildNodes().OfType<ExpressionStatementSyntax>().Single());
        }

        [Fact]
        public void Match_NestedBlock_False()
        {
            const string sourceText = "class C { void M() { { } } }";

            this.AssertMatch<MethodDeclarationSyntax>(
                sourceText,
                false,
                x => x.Body.NotNull().ChildNodes().OfType<BlockSyntax>().Single());
        }

        [Fact]
        public void Match_AutoProperty_False()
        {
            const string sourceText = "class C { int X { get; set; } }";

            this.AssertMatch<PropertyDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_AutoPropertyAccessor_False()
        {
            const string sourceText = "class C { int X { get; set; } }";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.AccessorList.NotNull().Accessors.First());
        }

        [Fact]
        public void Match_ReadOnlyProperty_False()
        {
            const string sourceText = "class C { int X => 5; }";

            this.AssertMatch<PropertyDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_ReadOnlyPropertyExpressionBody_False()
        {
            const string sourceText = "class C { int X => 5; }";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.ExpressionBody.NotNull());
        }

        [Fact]
        public void Match_GetSetBodyPropertyGetterBlock_False()
        {
            const string sourceText =
                "class C { int X { get { return 5; } set { System.Write(value); }}";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.AccessorList.NotNull().Accessors.First().Body.NotNull());
        }

        [Fact]
        public void Match_GetSetBodyPropertySetterBlock_False()
        {
            const string sourceText =
                "class C { int X { get { return 5; } set { System.Write(value); }}";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.AccessorList.NotNull().Accessors.Last().Body.NotNull());
        }

        [Fact]
        public void Match_GetOnlyAutoProperty_False()
        {
            const string sourceText = "class C { int X { get; } = 5; }";

            this.AssertMatch<PropertyDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_GetOnlyAutoPropertyGetter_False()
        {
            const string sourceText = "class C { int X { get; } = 5; }";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.AccessorList.NotNull().Accessors.First());
        }

        [Fact]
        public void Match_GetOnlyBodyProperty_False()
        {
            const string sourceText = "class C { int X { get { return 5; } }";

            this.AssertMatch<PropertyDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_GetOnlyBodyGetterBlock_False()
        {
            const string sourceText = "class C { int X { get { return 5; } }";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.AccessorList.NotNull().Accessors.First().Body.NotNull());
        }

        [Fact]
        public void Match_SetOnlyBodyProperty_False()
        {
            const string sourceText = "class C { int X { set { System.Console.Write(value); } }";

            this.AssertMatch<PropertyDeclarationSyntax>(sourceText, false);
        }

        [Fact]
        public void Match_SetOnlyBodyPropertySetter_False()
        {
            const string sourceText = "class C { int X { set { System.Console.Write(value); } }";

            this.AssertMatch<PropertyDeclarationSyntax>(
                sourceText,
                false,
                x => x.AccessorList.NotNull().Accessors.First().Body.NotNull());
        }
    }
}
