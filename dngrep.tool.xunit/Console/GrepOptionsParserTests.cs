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
    }
}
