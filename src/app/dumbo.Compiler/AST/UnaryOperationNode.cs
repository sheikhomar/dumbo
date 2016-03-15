namespace dumbo.Compiler.AST
{
    public class UnaryOperationNode : ExpressionNode
    {
        public UnaryOperatorType Operator;
        public ExpressionNode Expression;
        
    }
}