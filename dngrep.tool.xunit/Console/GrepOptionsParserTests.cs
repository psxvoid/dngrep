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

        public class ContainsTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsEmptyEnumerable()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Empty(result.Value.Contains);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-c", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.Contains);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-c", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.Contains);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--contains", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.Contains);
            }

            [Fact]
            public void ParseArguments_FullNameAndFooAndBar_ShouldParsTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--contains", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.Contains);
            }
        }

        public class ExcludeTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsEmptyEnumerable()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Empty(result.Value.Exclude);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-e", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.Exclude);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-e", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.Exclude);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--exclude", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.Exclude);
            }

            [Fact]
            public void ParseArguments_FullNameAndFooAndBar_ShouldParsTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--exclude", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.Exclude);
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

            [Fact(Skip = "https://github.com/commandlineparser/commandline/issues/316")]
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

            [Fact(Skip = "https://github.com/commandlineparser/commandline/issues/316")]
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

        public class HideNamespacesTests
        {
            [Fact]
            public void ParseArguments_Empty_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Equal(false, result.Value.HideNamespaces);
            }

            [Fact(Skip = "https://github.com/commandlineparser/commandline/issues/316")]
            public void ParseArguments_FullNameFlagOnly_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--hide-namespaces" });

                Assert.Equal(true, result.Value.HideNamespaces);
            }

            [Fact]
            public void ParseArguments_FullNameTrue_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--hide-namespaces", "true" });

                Assert.Equal(true, result.Value.HideNamespaces);
            }

            [Fact]
            public void ParseArguments_FullNameFalse_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--hide-namespaces", "false" });

                Assert.Equal(false, result.Value.HideNamespaces);
            }
        }
    }
}
