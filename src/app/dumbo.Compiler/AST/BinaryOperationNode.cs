using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class BinaryOperationNode : ExpressionNode
    {
        public BinaryOperationNode(ExpressionNode left, BinaryOperatorType operatorType, ExpressionNode right)
        {
            LeftOperand = left;
            Operator = operatorType;
            RightOperand = right;
        }

        public ExpressionNode LeftOperand { get; }
        public BinaryOperatorType Operator { get; }
        public ExpressionNode RightOperand { get; }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            LeftOperand.PrettyPrint(strBuilder);
            strBuilder.Append(" " + OperatorTranslator.BinaryOperatorTypeTranslator(Operator) + " ");
            RightOperand.PrettyPrint(strBuilder);
        }
    }
}