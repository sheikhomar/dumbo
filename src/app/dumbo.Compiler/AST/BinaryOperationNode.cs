using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
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

            if (leftOperandDesc.GetNumberOfTypes() == 1 || rightOperandDesc.GetNumberOfTypes() == 1)
            {
                lo = leftOperandDesc.GetFirst();
                ro = rightOperandDesc.GetFirst();
            }

            return EvaluateType(lo, Operator, ro);
        }

        private TypeDescriptor EvaluateType(HappyType leftOperand, BinaryOperatorType @operator, HappyType rightOperand)
        {
            bool check;

            switch (@operator)
            {
                case BinaryOperatorType.Plus:
                    if (leftOperand.Equals(rightOperand) && rightOperand == HappyType.Number)
                        return new TypeDescriptor(HappyType.Number);
                    else if (leftOperand.Equals(rightOperand) && rightOperand == HappyType.Text)
                        return new TypeDescriptor(HappyType.Text);
                    if ((leftOperand == HappyType.Text && rightOperand == HappyType.Number) ||
                        (leftOperand == HappyType.Number && rightOperand == HappyType.Text))
                        return new TypeDescriptor(HappyType.Text);
                    else
                        return new TypeDescriptor(HappyType.Error);
                case BinaryOperatorType.Minus:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Number, check);
                case BinaryOperatorType.Times:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Number, check);
                case BinaryOperatorType.Division:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Number, check);
                case BinaryOperatorType.Modulo:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Number, check);
                case BinaryOperatorType.Equals:
                    if(leftOperand == HappyType.Number)
                        check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    else
                        check = false;
                    return TypeAdder(HappyType.Boolean, check);
                case BinaryOperatorType.GreaterThan:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Boolean, check);
                case BinaryOperatorType.GreaterOrEqual:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Boolean, check);
                case BinaryOperatorType.LessThan:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Boolean, check);
                case BinaryOperatorType.LessOrEqual:
                    check = IsSameType(HappyType.Number, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Boolean, check);
                case BinaryOperatorType.Or:
                    check = IsSameType(HappyType.Boolean, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Boolean, check);
                case BinaryOperatorType.And:
                    check = IsSameType(HappyType.Boolean, leftOperand, rightOperand);
                    return TypeAdder(HappyType.Boolean, check);
                default:
                    throw new FormatException("Uknown Binary Operator");
            }
        }

        #region Helper functions
        private TypeDescriptor TypeAdder(HappyType typeToAdd, bool check)
        {
            if (check)
                return new TypeDescriptor(typeToAdd);
            else
                return new TypeDescriptor(HappyType.Error);
        }

        private bool IsSameType(HappyType checkType, HappyType leftOperand, HappyType rightOperand)
        {
            if (leftOperand == checkType && rightOperand == checkType)
                return true;
            return false;
        }
        #endregion

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            LeftOperand.CCAnalyse(analyser);
            RightOperand.CCAnalyse(analyser);
        }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}