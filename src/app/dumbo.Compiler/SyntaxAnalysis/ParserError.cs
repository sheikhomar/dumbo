namespace dumbo.Compiler.SyntaxAnalysis
{
    public abstract class ParserError
    {
        public abstract string GetErrorMessage();

        public override string ToString()
        {
            return GetErrorMessage();
        }
    }
}