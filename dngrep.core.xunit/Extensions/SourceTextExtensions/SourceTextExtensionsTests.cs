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
        public void GetSingleCharSpan_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                () => ((SourceText)null).GetSingleCharSpan(0, 0));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 26)]
        [InlineData(-1, 0)]
        [InlineData(1, 0)]
        public void GetSingleCharSpan_OutOfRange_ThrowsOutOfRange(int line, int charPos)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => this.tree.GetText().GetSingleCharSpan(line, charPos));
        }

        [Fact]
        public void GetSingleCharSpan_ZeroZero_EntireLine()
        {
            TextSpan result = this.tree.GetText().GetSingleCharSpan(0, 0);

            Assert.Equal(new TextSpan(0, 1), result);
        }

        [Fact]
        public void GetSingleCharSpan_ZeroZero_EntireLineString()
        {
            TextSpan result = this.tree.GetText().GetSingleCharSpan(0, 0);

            Assert.Equal("c", this.tree.GetText().ToString(result));
        }

        [Fact]
        public void GetSingleCharSpan_ZeroSix_ShouldReturnSpan()
        {
            TextSpan result = this.tree.GetText().GetSingleCharSpan(0, 6);

            Assert.Equal(new TextSpan(6, 1), result);
        }

        [Fact]
        public void GetSingleCharSpan_ZeroSix_ShouldReturnSpanString()
        {
            TextSpan result = this.tree.GetText().GetSingleCharSpan(0, 6);

            Assert.Equal("P", this.tree.GetText().ToString(result));
        }
    }
}
