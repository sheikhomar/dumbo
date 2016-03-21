namespace dumbo.Compiler.CCAnalysis
{
    public class CCError
    {
        public string Message { get; }
        public int Line { get; }
        public int Column { get; }

        public CCError(string message, int line, int column)
        {
            Message = message;
            Line = line;
            Column = column;
        }
    }
}