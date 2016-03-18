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
            HappyType lo = HappyType.Error, ro = HappyType.Error;

            if (leftOperandDesc.Types.Count == 1 || rightOperandDesc.Types.Count == 1)
            {
                lo = leftOperandDesc.GetFirst();
                ro = rightOperandDesc.GetFirst();
            }

            return EvaluateType(lo, Operator, ro);
        }

        private TypeDescriptor EvaluateType(HappyType leftOperand, BinaryOperatorType @operator, HappyType rightOperand)
        {
            var td = new TypeDescriptor();
            bool check;

            switch (@operator)
            {
                case BinaryOperatorType.Plus:
                    if (leftOperand.Equals(rightOperand) && rightOperand == HappyType.Number)
                        td.Add(HappyType.Number);
                    else if (leftOperand.Equals(rightOperand) && rightOperand == HappyType.Text)
                        td.Add(HappyType.Text);
                    else
                        td.Add(HappyType.Error);
                    break;
                case BinaryOperatorType.Minus:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Number, check);
                    break;
                case BinaryOperatorType.Times:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Number, check);
                    break;
                case BinaryOperatorType.Division:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Number, check);
                    break;
                case BinaryOperatorType.Modulo:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Number, check);
                    break;
                case BinaryOperatorType.Equals:
                    if(leftOperand == HappyType.Number)
                        check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    else if (leftOperand == HappyType.Boolean)
                        check = IsSameType(HappyType.Boolean, leftOperand, rightOperand);
                    else if (leftOperand == HappyType.Text)
                        check = IsSameType(HappyType.Text, leftOperand, rightOperand);
                    else
                        check = false;
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                case BinaryOperatorType.GreaterThan:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                case BinaryOperatorType.GreaterOrEqual:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                case BinaryOperatorType.LessThan:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                case BinaryOperatorType.LessOrEqual:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                case BinaryOperatorType.Or:
                    check = IsSameType(HappyType.Boolean, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                case BinaryOperatorType.And:
                    check = IsSameType(HappyType.Boolean, leftOperand, rightOperand);
                    TypeAdder(td, HappyType.Boolean, check);
                    break;
                default:
                    throw new FormatException("Uknown Binary Operator");
            }
            return td;
        }

        #region Helper functions
        private void TypeAdder(TypeDescriptor td, HappyType typeToAdd, bool check)
        {
            if (check)
                td.Add(typeToAdd);
            else
                td.Add(HappyType.Error);
        }

        private bool IsSameType(HappyType checkType, HappyType leftOperand, HappyType rightOperand)
        {
            if (leftOperand == checkType && rightOperand == checkType)
                return true;
            return false;
        }
        #endregion
    }
}