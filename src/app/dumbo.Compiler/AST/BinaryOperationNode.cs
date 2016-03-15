namespace dumbo.Compiler.AST
{
    public class BinaryOperationNode : ExpressionNode
    {
        public ExpressionNode LeftOperand;
        public BinaryOperatorType Operator;
        public ExpressionNode RightOperand;
    }
}