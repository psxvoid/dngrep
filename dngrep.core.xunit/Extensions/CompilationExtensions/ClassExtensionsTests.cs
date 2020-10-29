using System.Collections.Generic;
using dngrep.core.Extensions.CompilationExtensions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace dngrep.core.xunit.Extensions.CompilationExtensions
{
    public static class ClassExtensionsTests
    {
        public class TwoPublicOnePrivateClassesTests
        {
            private const string SourceCode = @"
                public class Cat
                {
                }

                public class Dog
                {
                }

                private class Ninja
                {
                }
            ";

            private readonly CSharpCompilation compilation;
            private readonly IReadOnlyCollection<string> names;

            public TwoPublicOnePrivateClassesTests()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
                this.names = this.compilation.GetClassNames();
            }

            [Fact]
            public void GetClassNames_ShouldGetPublicAndPrivateClasses()
            {
                Assert.Equal(3, this.names.Count);
            }

            [Fact]
            public void GetClassNames_ShouldGetCorrectClassNames()
            {
                Assert.Contains("Cat", this.names);
                Assert.Contains("Dog", this.names);
                Assert.Contains("Ninja", this.names);
            }
        }

        public class TwoClassesOneInterfaceTests
        {
            private const string SourceCode = @"
                public class Cat
                {
                }

                public class Dog
                {
                }

                public interface Ninja
                {
                }
            ";

            private readonly CSharpCompilation compilation;
            private readonly IReadOnlyCollection<string> names;

            public TwoClassesOneInterfaceTests()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
                this.names = this.compilation.GetClassNames();
            }

            [Fact]
            public void GetClassNames_ShouldNotGetInterface()
            {
                Assert.Equal(2, this.names.Count);
            }

            [Fact]
            public void GetClassNames_ShouldGetCorrectClassNames()
            {
                Assert.Contains("Cat", this.names);
                Assert.Contains("Dog", this.names);
            }
        }

        public class ThreeClassesOneAbstractTests
        {
            private const string SourceCode = @"
                public class Cat
                {
                }

                public class Dog
                {
                }

                public abstract class Ninja
                {
                }
            ";

            private readonly CSharpCompilation compilation;
            private readonly IReadOnlyCollection<string> names;

            public ThreeClassesOneAbstractTests()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
                this.names = this.compilation.GetClassNames();
            }

            [Fact]
            public void GetClassNames_ShouldIncludeAbstractClasses()
            {
                Assert.Equal(3, this.names.Count);
            }

            [Fact]
            public void GetClassNames_ShouldGetCorrectClassNames()
            {
                Assert.Contains("Cat", this.names);
                Assert.Contains("Dog", this.names);
                Assert.Contains("Ninja", this.names);
            }
        }
    }
}
