using System.IO;

namespace dumbo.Compiler
{
    public class Parser : IParser
    {
        public Parser(string parseTable)
        {
            
        }

        public AST Parse(TextReader reader)
        {
            return new AST();
        }
    }
}