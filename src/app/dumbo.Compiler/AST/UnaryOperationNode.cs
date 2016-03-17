using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class UnaryOperationNode : ExpressionNode
    {
        public UnaryOperationNode(UnaryOperatorType @operator, ExpressionNode expression)
        {
            Operator = @operator;
            Expression = expression;
        }

        public UnaryOperatorType Operator { get; }
        public ExpressionNode Expression { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(" " + OperatorTranslator.UnaryOperatorTypeTranslator(Operator) + " ");
            Expression.PrettyPrint(prettyPrinter);
        }
    }
}