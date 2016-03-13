namespace dumbo.Compiler.SyntaxAnalysis
{
    public class SyntaxError : ParserError
    {
        public int Column { get; }
        public string ExpectedTokens { get; }
        public int Line { get; }
        public string ReadToken { get; }

        public SyntaxError(int line, int column, string readToken, string expectedTokens)
        {
            Line = line;
            Column = column;
            ReadToken = readToken;
            ExpectedTokens = expectedTokens;
        }

        public override string GetErrorMessage()
        {
            return $"Syntax error in line {Line}:{Column}\n Read: {ReadToken}\n Expecting: {ExpectedTokens}\n";
        }
    }
}