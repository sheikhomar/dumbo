namespace dumbo.Compiler.AST
{
    public class ArrayIdentifierNode : IdentifierNode
    {
        public ExpressionListNode Indices { get; }

        public ArrayIdentifierNode(string name, ExpressionListNode indices, SourcePosition sourcePosition) : base(name, sourcePosition)
        {
            Indices = indices;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}