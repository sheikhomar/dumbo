namespace dumbo.Compiler.AST
{
    public class ArrayIdentifierNode : IdentifierNode
    {
        public ExpressionListNode Sizes { get; }

        public ArrayIdentifierNode(string name, ExpressionListNode sizes, SourcePosition sourcePosition) : base(name, sourcePosition)
        {
            Sizes = sizes;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}