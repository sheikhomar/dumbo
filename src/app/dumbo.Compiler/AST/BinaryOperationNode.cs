namespace dumbo.Compiler.AST
{
    public class BinaryOperationNode : ExpressionNode
    {
        public BinaryOperationNode(ExpressionNode left, BinaryOperatorType operatorType, ExpressionNode right, SourcePosition sourcePosition)
        {
            LeftOperand = left;
            Operator = operatorType;
            RightOperand = right;
            SourcePosition = sourcePosition;
        }

        public ExpressionNode LeftOperand { get; }
        public BinaryOperatorType Operator { get; }
        public ExpressionNode RightOperand { get; }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}