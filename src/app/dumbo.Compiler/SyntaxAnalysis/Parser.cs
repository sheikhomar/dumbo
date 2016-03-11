using System.IO;
using GOLD;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class Parser : IParser
    {
        public Reduction Root; //Store the top of the tree
        public string FailMessage;
        private readonly string _parseTablePath;

        public Parser(string parseTablePath)
        {
            _parseTablePath = parseTablePath;
            
        }

        public ParserResult Parse(TextReader reader)
        {
            GOLD.Parser parser = new GOLD.Parser();

            if (!parser.LoadTables(_parseTablePath))
                return new ParserResult($"Cannot load parse table \"{_parseTablePath}\".");

            if (!parser.Open(reader))
                return new ParserResult($"Cannot open file to parse.");
            
            return ParseInternal(parser);
        }

        private ParserResult ParseInternal(GOLD.Parser parser)
        {
            ParserResult result = new ParserResult();

            // Based on code sample project. 
            parser.TrimReductions = false;
            bool accepted = false; //Was the parse successful?
            
            var done = false;
            while (!done)
            {
                var response = parser.Parse();

                switch (response)
                {
                    case GOLD.ParseMessage.LexicalError:
                        //Cannot recognize token
                        FailMessage = "Lexical Error: \n " +
                                      "Position: " + parser.CurrentPosition().Line + ", " +
                                      parser.CurrentPosition().Column + " \n " +
                                      "Read: " + parser.CurrentToken().Data + " \n ";
                        done = true;
                        break;

                    case GOLD.ParseMessage.SyntaxError:
                        //Expecting a different token
                        FailMessage = "Syntax Error:\n " +
                                      "Position: " + parser.CurrentPosition().Line + ", " +
                                      parser.CurrentPosition().Column + "\n  " +
                                      "Read: " + parser.CurrentToken().Data + "\n  " +
                                      "Expecting: " + parser.ExpectedSymbols().Text();
                        done = true;
                        break;

                    case GOLD.ParseMessage.Reduction:
                        //For this project, we will let the parser build a tree of Reduction objects
                        //parser.CurrentReduction = CreateNewObject(parser.CurrentReduction);
                        break;

                    case GOLD.ParseMessage.Accept:
                        //Accepted!
                        Root = (GOLD.Reduction)parser.CurrentReduction;
                        //The root node!                                  
                        done = true;
                        accepted = true;
                        break;

                    case GOLD.ParseMessage.TokenRead:
                        //You don't have to do anything here.
                        break;

                    case GOLD.ParseMessage.InternalError:
                        //INTERNAL ERROR! Something is horribly wrong.
                        done = true;
                        break;

                    case GOLD.ParseMessage.NotLoadedError:
                        //This error occurs if the CGT was not loaded.                   
                        FailMessage = "Tables not loaded";
                        done = true;
                        break;

                    case GOLD.ParseMessage.GroupError:
                        //GROUP ERROR! Unexpected end of file
                        FailMessage = "Runaway group";
                        done = true;
                        break;
                }
            } //while
            
            return result;

        }
    }
}