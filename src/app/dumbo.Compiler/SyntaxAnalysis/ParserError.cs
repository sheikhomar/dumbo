namespace dumbo.Compiler.SyntaxAnalysis
{
    public abstract class ParserError
    {
        public abstract string GetErrorMessage();
        public abstract int LineNumber { get; }
        public abstract int Column { get; }

        public override string ToString()
        {
            return GetErrorMessage();
        }
    }
}
