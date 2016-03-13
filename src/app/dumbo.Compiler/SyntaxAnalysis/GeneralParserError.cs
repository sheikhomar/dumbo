namespace dumbo.Compiler.SyntaxAnalysis
{
    public class GeneralParserError: ParserError
    {
        private readonly string _message;

        public GeneralParserError(string message)
        {
            _message = message;
        }

        public override string GetErrorMessage()
        {
            return _message;
        }
    }
}