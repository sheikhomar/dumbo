namespace dumbo.Compiler.AST
{
    public abstract class ExpressionNode : BaseNode
    {
        public TypeDescriptor InferredType { get; set; }
    }
}