namespace dumbo.Compiler.AST
{
    public class LiteralValueNode : ValueNode
    {
        public LiteralValueNode(string value, HappyType type) : base(type)
        {
            Value = value;
        }

        public string Value { get; }
    }
}