namespace dumbo.Compiler.AST
{
    public class ArrayIdentifierNode : IdentifierNode
    {
        public ExpressionListNode Sizes { get; }

        public ArrayIdentifierNode(string name, ExpressionListNode sizes, SourcePosition sourcePosition) : base(name, sourcePosition)
        {
            Sizes = sizes;
        }
    }
}