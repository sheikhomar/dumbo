using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

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

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            TypeDescriptor td = Expression.GetHappyType(symbolTable);

            TypeDescriptor returnTypeDesc = new TypeDescriptor();

            if (td.Types.Count == 1)
                return EvaluateType(td.GetFirst());
            else
            {
                returnTypeDesc.Add(HappyType.Error);
                return returnTypeDesc;
            }
        }

        private TypeDescriptor EvaluateType(HappyType type)
        {
            TypeDescriptor td = new TypeDescriptor();

            switch (Operator)
            {
                case UnaryOperatorType.Not:
                    TypeAdder(td, type, HappyType.Boolean);
                    break;
                case UnaryOperatorType.Minus:
                    TypeAdder(td, type, HappyType.Number);
                    break;
                case UnaryOperatorType.Plus:
                    TypeAdder(td, type, HappyType.Number);
                    break;
                default:
                    throw new FormatException("Uknown Unary Operator");
            }

            return td;
        }

        private void TypeAdder(TypeDescriptor td, HappyType type1, HappyType type2)
        {
            if (type1 == type2)
                td.Add(type1);
            else
                td.Add(HappyType.Error);
        }
    }
}