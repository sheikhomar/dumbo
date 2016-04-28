namespace dumbo.Compiler.AST
{
    public class ArrayValueNode : ValueNode
    {
        public ArrayValueNode(ArrayTypeNode type) : base(type)
        {
            ArrayType = type;
            Values = new ExpressionListNode();
        }

        public ArrayTypeNode ArrayType { get; }

        public ExpressionListNode Values { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}