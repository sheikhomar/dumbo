namespace dumbo.Compiler.AST
{
    public abstract class BaseNode : IVisitable
    {
        protected BaseNode()
        {
            SourcePosition = new SourcePosition(0, 0, 0, 0);
        }

        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public SourcePosition SourcePosition { get; set; }

        public abstract T Accept<T,K>(IVisitor<T,K> visitor, K arg);
    }
}