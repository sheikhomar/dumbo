using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            LeftOperand.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(" " + OperatorTranslator.BinaryOperatorTypeTranslator(Operator) + " ");
            RightOperand.PrettyPrint(prettyPrinter);
        }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            throw new NotImplementedException();
        }
    }
}