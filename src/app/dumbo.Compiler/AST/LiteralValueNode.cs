namespace dumbo.Compiler.AST
{
    public class LiteralValueNode : ValueNode
    {
        public LiteralValueNode(string value, TypeNode type, SourcePosition sourcePosition) : base(type)
        {
            Value = value;
            SourcePosition = sourcePosition;
        }

        public string Value { get; }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}