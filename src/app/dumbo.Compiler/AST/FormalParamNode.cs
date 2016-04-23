namespace dumbo.Compiler.AST
{
    public class FormalParamNode : BaseNode, IVariableDeclNode
    {
        public FormalParamNode(string name, TypeNode type, SourcePosition sourcePosition)
        {
            Name = name;
            Type = type;
            SourcePosition = sourcePosition;
        }

        public string Name { get; }
        public TypeNode Type { get; }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}