namespace dumbo.Compiler.AST
{
    public class IdentifierNode : ExpressionNode
    {
        public IdentifierNode(string name, SourcePosition sourcePosition)
        {
            Name = name.ToLower();
            SourcePosition = sourcePosition;
        }

        public string Name { get; }
        public IVariableDeclNode DeclarationNode { get; set; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}