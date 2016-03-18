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
            TypeDescriptor leftOperandDesc = LeftOperand.GetHappyType(symbolTable);
            TypeDescriptor rightOperandDesc = RightOperand.GetHappyType(symbolTable);

        }

        private TypeDescriptor EvaluateType(HappyType leftOperand, BinaryOperatorType @operator, HappyType rightOperand)
        {
            switch (@operator)
            {
                case BinaryOperatorType.Plus:
                    // TODO: Add something with numbers and texts
                    break;
                case BinaryOperatorType.Minus:
                    if (leftOperand.Equals(rightOperand) && leftOperand.Equals(HappyType.Number))
                        return HappyType.Number;
                    else
                        
                    break;
                case BinaryOperatorType.Times:
                    break;
                case BinaryOperatorType.Division:
                    break;
                case BinaryOperatorType.Modulo:
                    break;
                case BinaryOperatorType.Equals:
                    break;
                case BinaryOperatorType.GreaterThan:
                    break;
                case BinaryOperatorType.GreaterOrEqual:
                    break;
                case BinaryOperatorType.LessThan:
                    break;
                case BinaryOperatorType.LessOrEqual:
                    break;
                case BinaryOperatorType.Or:
                    break;
                case BinaryOperatorType.And:
                    break;
                default:
                    break;
            }
        }
    }
}