using System;
using CommandLine;
using dngrep.core.Queries.Specifiers;
using dngrep.tool.Core.Options;
using dngrep.tool.Core.Output.Presenters;
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
            public void ParseArguments_FullNameAndFooAndBar_ShouldParseTwoNames()
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
            public void ParseArguments_FullNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--exclude", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.Exclude);
            }
        }

        public class EnableRegexpTests
        {
            [Fact]
            public void ParseArguments_Empty_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Equal(false, result.Value.EnableRegexp);
            }

            [Fact(Skip = "https://github.com/commandlineparser/commandline/issues/316")]
            public void ParseArguments_ShortNameFlagOnly_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--r" });

                Assert.Equal(true, result.Value.EnableRegexp);
            }

            [Fact]
            public void ParseArguments_ShortNameTrue_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--r", "true" });

                Assert.Equal(true, result.Value.EnableRegexp);
            }

            [Fact]
            public void ParseArguments_ShortNameFalse_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--r", "false" });

                Assert.Equal(false, result.Value.EnableRegexp);
            }

            [Fact(Skip = "https://github.com/commandlineparser/commandline/issues/316")]
            public void ParseArguments_FullNameFlagOnly_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--regexp" });

                Assert.Equal(true, result.Value.EnableRegexp);
            }

            [Fact]
            public void ParseArguments_FullNameTrue_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--regexp", "true" });

                Assert.Equal(true, result.Value.EnableRegexp);
            }

            [Fact]
            public void ParseArguments_FullNameFalse_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--regexp", "false" });

                Assert.Equal(false, result.Value.EnableRegexp);
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

        public class ScopeContainsTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsEmptyEnumerable()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Empty(result.Value.ScopeContains);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-C", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.ScopeContains);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-C", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.ScopeContains);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--scope-contains", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.ScopeContains);
            }

            [Fact]
            public void ParseArguments_FullNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--scope-contains", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.ScopeContains);
            }
        }

        public class ScopeExcludeTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsEmptyEnumerable()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Empty(result.Value.ScopeExclude);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-E", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.ScopeExclude);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-E", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.ScopeExclude);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--scope-exclude", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.ScopeExclude);
            }

            [Fact]
            public void ParseArguments_FullNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--scope-exclude", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.ScopeExclude);
            }
        }

        public class PathContainsTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsEmptyEnumerable()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Empty(result.Value.PathContains);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-p", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.PathContains);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-p", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.PathContains);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--path-contains", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.PathContains);
            }

            [Fact]
            public void ParseArguments_FullNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--path-contains", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.PathContains);
            }
        }

        public class PathExcludeTests
        {
            [Fact]
            public void ParseArguments_Empty_ShouldParseAsEmptyEnumerable()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Empty(result.Value.PathExclude);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-P", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.PathExclude);
            }

            [Fact]
            public void ParseArguments_ShortNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-P", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.PathExclude);
            }

            [Fact]
            public void ParseArguments_FullNameAndFoo_ShouldParseSingleName()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--path-exclude", "Foo" });

                Assert.Equal(new[] { "Foo" }, result.Value.PathExclude);
            }

            [Fact]
            public void ParseArguments_FullNameAndFooAndBar_ShouldParseTwoNames()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--path-exclude", "Foo", "Bar" });

                Assert.Equal(new[] { "Foo", "Bar" }, result.Value.PathExclude);
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
                    Parser.ParseArguments<GrepOptions>(new[] { "-n" });

                Assert.Equal(true, result.Value.ShowFullName);
            }

            [Fact]
            public void ParseArguments_ShortNameFalse_False()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-n", "false" });

                Assert.Equal(false, result.Value.ShowFullName);
            }

            [Fact]
            public void ParseArguments_ShortNameTrue_True()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-n", "true" });

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

        public class OutputTypeTests
        {
            [Fact]
            public void ParseArguments_Empty_Search()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(Array.Empty<string>());

                Assert.Equal(PresenterKind.Search, result.Value.OutputType);
            }
            
            [Fact]
            public void ParseArguments_ShortNameAndSearch_Search()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-o", "search" });

                Assert.Equal(PresenterKind.Search, result.Value.OutputType);
            }

            [Fact]
            public void ParseArguments_ShortNameAndStatistics_Statistics()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "-o", "statistics" });

                Assert.Equal(PresenterKind.Statistics, result.Value.OutputType);
            }

            [Fact]
            public void ParseArguments_FullNameAndSearch_Search()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--output-type", "search" });

                Assert.Equal(PresenterKind.Search, result.Value.OutputType);
            }

            [Fact]
            public void ParseArguments_FullNameAndStatistics_Statistics()
            {
                ParserResult<GrepOptions>? result =
                    Parser.ParseArguments<GrepOptions>(new[] { "--output-type", "statistics" });

                Assert.Equal(PresenterKind.Statistics, result.Value.OutputType);
            }
        }
    }
}
