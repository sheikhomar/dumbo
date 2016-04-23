namespace dumbo.Compiler.AST
{
    public class ArrayTypeNode : TypeNode
    {
        public PrimitiveTypeNode Type { get; set; }
        public ExpressionListNode Sizes { get; set; }

        public ArrayTypeNode(PrimitiveTypeNode type, ExpressionListNode sizes, SourcePosition srcPos)
        {
            Type = type;
            Sizes = sizes;
            SourcePosition = srcPos;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            throw new System.NotImplementedException();
        }
    }
}