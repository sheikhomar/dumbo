namespace dumbo.Compiler.AST
{
    public class UnaryOperationNode : ExpressionNode
    {
        public UnaryOperationNode(UnaryOperatorType @operator, ExpressionNode expression, SourcePosition sourcePosition)
        {
            Operator = @operator;
            Expression = expression;
            SourcePosition = sourcePosition;
        }

        public UnaryOperatorType Operator { get; }
        public ExpressionNode Expression { get; }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}