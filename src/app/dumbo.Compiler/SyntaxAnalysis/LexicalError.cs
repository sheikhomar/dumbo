using System.Text;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class LexicalError : ParserError
    {
        public string CurrentToken { get; }
        public override int LineNumber { get; }
        public override int Column { get; }

        public LexicalError(int line, int column, string currentToken)
        {
            LineNumber = line;
            Column = column;
            CurrentToken = currentToken;
        }

        public override string GetErrorMessage()
        {
            return $"Lexical error on line: {LineNumber} [Col {Column}]\nCurrent token: {CurrentToken}\n";
        }


    }
}
