using System;
using System.Diagnostics;
using System.IO;

namespace dumbo.Compiler.SyntaxAnalysis
{
    // Based on sample project that can be downloaded
    // from http://goldparser.org/engine/5/net/index.htm
    public class Parser : IParser
    {
        private readonly string _parseTablePath;

        public Parser(string parseTablePath)
        {
            _parseTablePath = parseTablePath;
        }

        public ParserResult Parse(TextReader reader)
        {
            var parser = new GOLD.Parser();

            if (!parser.LoadTables(_parseTablePath))
                return new ParserResult($"Cannot load parse table \"{_parseTablePath}\".");

            if (!parser.Open(reader))
                return new ParserResult($"Cannot open file to parse.");

            // From the sample code.
            parser.TrimReductions = false;

            return ParseInternal(parser);
        }

        private ParserResult ParseInternal(GOLD.Parser parser)
        {
            ParserResult result = new ParserResult();

            var done = false;
            while (!done)
            {
                var response = parser.Parse();

                switch (response)
                {
                    case GOLD.ParseMessage.TokenRead:
                        var token = parser.CurrentToken();
                        token.Data = new TokenData(parser.CurrentPosition(), token.Data);
                        break;

                    case GOLD.ParseMessage.Reduction:
                        break;

                    case GOLD.ParseMessage.Accept:
                        var root = (GOLD.Reduction)parser.CurrentReduction;
                        var ast = new AbstractSyntaxTreeBuilder().Build(root);
                        result.Root = ast;
                        done = true;
                        break;

                    case GOLD.ParseMessage.LexicalError:
                        //Cannot recognize token
                        AppendLexicalError(parser, result);
                        parser.DiscardCurrentToken();
                        break;

                    case GOLD.ParseMessage.SyntaxError:
                        //Expecting a different token
                        AppendSyntaxError(parser, result);
                        done = true;
                        break;

                    case GOLD.ParseMessage.InternalError:
                        result.AddGeneralError("Internal error in external library!");
                        done = true;
                        break;

                    case GOLD.ParseMessage.NotLoadedError:
                        result.AddGeneralError("Parse table is not loaded.");
                        done = true;
                        break;

                    case GOLD.ParseMessage.GroupError:
                        result.AddGeneralError("Unexpected end of file.");
                        done = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return result;
        }

        private void AppendSyntaxError(GOLD.Parser parser, ParserResult result)
        {
            var position = parser.CurrentPosition();
            var readToken = GetCurrentToken(parser);
            var expectedTokens = parser.ExpectedSymbols().Text();
            var error = new SyntaxError(position.Line + 1, position.Column + 1, readToken, expectedTokens);
            result.AddSyntaxError(error);
        }

        private void AppendLexicalError(GOLD.Parser parser, ParserResult result)
        {
            var position = parser.CurrentPosition();
            var currentToken = GetCurrentToken(parser);
            var error = new LexicalError(position.Line + 1, position.Column + 1, currentToken);
            result.AddLexicalError(error);
        }

        private string GetCurrentToken(GOLD.Parser parser)
        {
            return (parser.CurrentToken().Data as TokenData).Spelling;
        }
    }
}