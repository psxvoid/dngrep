using System;
using CommandLine;
using dngrep.core.Queries.Specifiers;
using dngrep.tool.Core.Options;
using Xunit;
using AbstractParser = dngrep.tool.Abstractions.CommandLine.Parser;

namespace dngrep.tool.xunit.Console
{
    public static class GrepOptionsParserTests
    {
        private static readonly Parser Parser = AbstractParser.Default();

        public class TargetTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsAny()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Equal(QueryTarget.Any, result.Value.Target);
            }

            [Fact]
            public void ParseArguments_ShortNameAndClass_ShouldParseAsClass()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-t", "class" });

                Assert.Equal(QueryTarget.Class, result.Value.Target);
            }

            [Fact]
            public void ParseArguments_FullNameAndClass_ShouldParseAsClass()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--target", "class" });

                Assert.Equal(QueryTarget.Class, result.Value.Target);
            }
        }

        public class TargetNameTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsNull()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Null(result.Value.TargetName);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseAsClass()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-c", "Foo" });

                Assert.Equal("Foo", result.Value.TargetName);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseAsClass()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--contains", "Foo" });

                Assert.Equal("Foo", result.Value.TargetName);
            }
        }

        public class ScopeTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsNone()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Equal(QueryTargetScope.None, result.Value.Scope);
            }

            [Fact]
            public void ParseArguments_ShortNameAndNamespace_ShouldParseAsNamespace()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-s", "namespace" });

                Assert.Equal(QueryTargetScope.Namespace, result.Value.Scope);
            }

            [Fact]
            public void ParseArguments_FullNameAndNamespace_ShouldParseAsNamespace()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--scope", "namespace" });

                Assert.Equal(QueryTargetScope.Namespace, result.Value.Scope);
            }
        }

        public class ShowFullNameTests
        {
            [Fact]
            public void ParseArguments_Empty_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Equal(true, result.Value.ShowFullName);
            }

            [Fact(Skip="https://github.com/commandlineparser/commandline/issues/316")]
            public void ParseArguments_ShortNameFlagOnly_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-f" });

                Assert.Equal(true, result.Value.ShowFullName);
            }
            
            [Fact]
            public void ParseArguments_ShortNameFalse_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-f", "false" });

                Assert.Equal(false, result.Value.ShowFullName);
            }
            
            [Fact]
            public void ParseArguments_ShortNameTrue_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-f", "true" });

                Assert.Equal(true, result.Value.ShowFullName);
            }

            [Fact(Skip="https://github.com/commandlineparser/commandline/issues/316")]
            public void ParseArguments_FullNameFlagOnly_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--show-full-name" });

                Assert.Equal(true, result.Value.ShowFullName);
            }
            
            [Fact]
            public void ParseArguments_FullNameFalse_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--show-full-name", "false" });

                Assert.Equal(false, result.Value.ShowFullName);
            }
            
            [Fact]
            public void ParseArguments_FullNameTrue_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--show-full-name", "true" });

                Assert.Equal(true, result.Value.ShowFullName);
            }
        }
    }
}
