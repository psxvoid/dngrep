using System.Collections.Generic;
using dngrep.core.Extensions.CompilationExtensions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace dngrep.core.xunit.Extensions.CompilationExtensions
{
    public static class MethodExtensionsTests
    {
        public class OneInstanceOneExtensionMethod
        {
            private const string SourceCode = @"
                public class AirPlain
                {
                    public void StartEngine() {
                    }
                }

                public static class AirPlainExtensions
                {
                    public static void TakePhoto(this AirPlain plain) {
                    }
                }
            ";

            private readonly CSharpCompilation compilation;
            private readonly IReadOnlyCollection<string> names;

            public OneInstanceOneExtensionMethod()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
                this.names = this.compilation.GetMethodNames();
            }

            [Fact]
            public void GetClassNames_ShouldGetPublicAndPrivateClasses()
            {
                Assert.Equal(2, this.names.Count);
            }

            [Fact]
            public void GetClassNames_ShouldGetCorrectClassNames()
            {
                Assert.Contains("StartEngine", this.names);
                Assert.Contains("TakePhoto", this.names);
            }
        }

        public class MethodInNestedClass
        {
            private const string SourceCode = @"
                public class AirPlain
                {
                    public void StartEngine() {
                    }

                    public class Engine()
                    {
                        public void AddOil(int amount)
                        {
                        }
                    }
                }
            ";

            private readonly CSharpCompilation compilation;
            private readonly IReadOnlyCollection<string> names;

            public MethodInNestedClass()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
                this.names = this.compilation.GetMethodNames();
            }

            [Fact]
            public void GetClassNames_ShouldGetPublicAndPrivateClasses()
            {
                Assert.Equal(2, this.names.Count);
            }

            [Fact]
            public void GetClassNames_ShouldGetCorrectClassNames()
            {
                Assert.Contains("StartEngine", this.names);
                Assert.Contains("AddOil", this.names);
            }
        }
    }
}
