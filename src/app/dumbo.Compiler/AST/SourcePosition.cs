namespace dumbo.Compiler.AST
{
    public class SourcePosition
    {
        public SourcePosition(int startLine, int startColumn, int endLine, int endColumn)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        public SourcePosition(BaseNode first, BaseNode second)
        {
            StartLine = first.SourcePosition.StartLine;
            StartColumn = first.SourcePosition.StartColumn;
            EndLine = second.SourcePosition.EndLine;
            EndColumn = second.SourcePosition.EndColumn;
        }

        public int StartLine { get; }
        public int StartColumn { get; }
        public int EndLine { get; }
        public int EndColumn { get; }

        public override string ToString()
        {
            return $"({StartLine},{StartColumn}) - ({EndLine}, {EndColumn})";
        }
    }
}