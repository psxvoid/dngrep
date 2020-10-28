using System;
using System.Collections.Generic;
using CLParser = CommandLine.Parser;

namespace dngrep.tool.Abstractions.CommandLine
{
    public interface IParser
    {
        IParserResult<T> ParseArguments<T>(IEnumerable<string> args);
        IParserResult<T> ParseArguments<T>(Func<T> factory, IEnumerable<string> args);
        IParserResult<object> ParseArguments(IEnumerable<string> args, params Type[] types);
    }

    public class Parser : IParser
    {
        private readonly CLParser parser;

        public Parser(CLParser parser)
        {
            this.parser = parser;
        }

        public IParserResult<T> ParseArguments<T>(IEnumerable<string> args)
        {
            return new ParserResult<T>(this.parser.ParseArguments<T>(args));
        }

        public IParserResult<T> ParseArguments<T>(Func<T> factory, IEnumerable<string> args)
        {
            return new ParserResult<T>(this.parser.ParseArguments(factory, args));
        }

        public IParserResult<object> ParseArguments(IEnumerable<string> args, params Type[] types)
        {
            return new ParserResult<object>(this.parser.ParseArguments(args, types));
        }
    }
}
