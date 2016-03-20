namespace dumbo.Compiler.SyntaxAnalysis
{
    public class SyntaxError : ParserError
    {
        public string ExpectedTokens { get; }
        public string ReadToken { get; }
        public override int Column { get; }
        public override int LineNumber { get; }

        public SyntaxError(int line, int column, string readToken, string expectedTokens)
        {
            LineNumber = line;
            Column = column;
            ReadToken = readToken;
            ExpectedTokens = expectedTokens;
        }

        public override string GetErrorMessage()
        {
            return $"Syntax error in line {LineNumber} [Col {Column}]\nRead: {ReadToken}\nExpecting: {ExpectedTokens}\n";
        }

    }
}