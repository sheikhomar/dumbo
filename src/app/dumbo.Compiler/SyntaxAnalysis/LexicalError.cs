using System.Text;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class LexicalError : ParserError
    {
        public int Column { get; }
        public int Line { get; }
        public string CurrentToken { get; }

        public LexicalError(int line, int column, string currentToken)
        {
            Line = line;
            Column = column;
            CurrentToken = currentToken;
        }

        public override string GetErrorMessage()
        {
            return $"Lexical error on line: {Line}:{Column}\n Current token: {CurrentToken}\n";
        }
    }
}