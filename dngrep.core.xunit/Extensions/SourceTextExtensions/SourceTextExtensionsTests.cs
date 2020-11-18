using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using dngrep.core.Extensions.SourceTextExtensions;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace dngrep.core.xunit.Extensions.SourceTextExtensions
{
    public class SourceTextExtensionsTests
    {
        private const string ClassString = "class Person { int age; }";

        private readonly SyntaxTree tree;

        public SourceTextExtensionsTests()
        {
            this.tree = CSharpSyntaxTree.ParseText(ClassString);
        }
        
        [Fact]
        public void GetLineSpanAtPosition_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                () => ((SourceText)null).GetLineSpanAtPosition(0, 0));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 26)]
        [InlineData(-1, 0)]
        [InlineData(1, 0)]
        public void GetLineSpanAtPosition_OutOfRange_ThrowsOutOfRange(int line, int charPos)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => this.tree.GetText().GetLineSpanAtPosition(line, charPos));
        }

        [Fact]
        public void GetLineSpanAtPosition_ZeroZero_EntireLine()
        {
            TextSpan result = this.tree.GetText().GetLineSpanAtPosition(0, 0);

            Assert.Equal(new TextSpan(0, ClassString.Length), result);
        }

        [Fact]
        public void GetLineSpanAtPosition_ZeroZero_EntireLineString()
        {
            TextSpan result = this.tree.GetText().GetLineSpanAtPosition(0, 0);

            Assert.Equal(ClassString, this.tree.GetText().ToString(result));
        }

        [Fact]
        public void GetLineSpanAtPosition_ZeroSix_ShouldReturnSpan()
        {
            TextSpan result = this.tree.GetText().GetLineSpanAtPosition(0, 6);

            Assert.Equal(new TextSpan(6, ClassString.Length - 6), result);
        }

        [Fact]
        public void GetLineSpanAtPosition_ZeroSix_ShouldReturnSpanString()
        {
            TextSpan result = this.tree.GetText().GetLineSpanAtPosition(0, 6);

            Assert.Equal("Person { int age; }", this.tree.GetText().ToString(result));
        }
    }
}
