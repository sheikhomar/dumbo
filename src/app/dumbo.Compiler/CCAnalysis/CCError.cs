namespace dumbo.Compiler.CCAnalysis
{
    public class CCError
    {
        public string Message { get; }
        public int Line { get; }
        public int Count { get; }

        public CCError(string message, int line, int count)
        {
            Message = message;
            Line = line;
            Count = count;
        }
    }
}