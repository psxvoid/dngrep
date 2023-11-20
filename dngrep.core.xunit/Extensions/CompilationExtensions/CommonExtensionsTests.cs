using System.Collections.Generic;
using dngrep.core.Extensions.CompilationExtensions;
using dngrep.core.xunit.TestHelpers;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace dngrep.core.xunit.Extensions.CompilationExtensions
{
    public static class CommonExtensionsTests
    {
        public class EachSupportedType
        {
            private const string SourceCode = @"
                using System;

                public interface IAnimal {
                }

                public enum WeatherState {
                    Sunny = 0
                }

                public struct Sun {
                }

                public class Cat : IAnimal
                {
                    private int lives = 9;
                    public int RemainingLives => this.lives;

                    public event EventHandler<EventArgs> OnWakeUp;

                    public string GetPurrText()
                    {
                        var catName = ""Felix"";
                        return catName + "" is purring"";
                    }
                }
            ";

            private readonly CSharpCompilation compilation;
            private readonly IReadOnlyCollection<string> names;

            public EachSupportedType()
            {
                this.compilation = TestCompiler.Compile(SourceCode);
                this.names = this.compilation.GetCommonSymbolNames();
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetInterfaceName()
            {
                Assert.Contains("IAnimal", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetClassName()
            {
                Assert.Contains("Cat", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetFieldName()
            {
                Assert.Contains("lives", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetPropertyName()
            {
                Assert.Contains("RemainingLives", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetMethodName()
            {
                Assert.Contains("GetPurrText", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetVariableName()
            {
                Assert.Contains("catName", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetEventName()
            {
                Assert.Contains("OnWakeUp", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetEnumName()
            {
                Assert.Contains("WeatherState", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetEnumMemberName()
            {
                Assert.Contains("Sunny", this.names);
            }

            [Fact]
            public void GetCommonSymbolNames_ShouldGetStructName()
            {
                Assert.Contains("Sun", this.names);
            }
        }
    }
}
