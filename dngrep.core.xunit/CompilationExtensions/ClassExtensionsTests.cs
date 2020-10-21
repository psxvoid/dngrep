using System.Collections.Generic;
using System.Linq;
using dngrep.core.CompilationExtensions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace dngrep.core.xunit
{
    public static class ClassExtensionsTests
    {
        public class SimpleClassExtensionsTests
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

            public SimpleClassExtensionsTests()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
            }
            
            [Fact]
            public void GetClassNames_ShouldGetPublicAndPrivateClasses()
            {
                IEnumerable<string> names = this.compilation.GetClassNames();

                Assert.Equal(3, names.Count());
            }

            [Fact]
            public void GetClassNames_ShouldGetCorrectClassNames()
            {
                IEnumerable<string> names = this.compilation.GetClassNames();

                Assert.Contains("Cat", names);
                Assert.Contains("Dog", names);
                Assert.Contains("Ninja", names);
            }
        }
    }
}
