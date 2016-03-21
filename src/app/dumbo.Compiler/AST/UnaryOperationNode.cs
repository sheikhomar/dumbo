using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
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

            if (td.Types.Count == 1)
                return EvaluateType(td.GetFirst());
            else
            {
                return new TypeDescriptor(HappyType.Error);
            }
        }

        private TypeDescriptor EvaluateType(HappyType type)
        {
            switch (Operator)
            {
                case UnaryOperatorType.Not:
                    return TypeAdder(type, HappyType.Boolean);
                case UnaryOperatorType.Minus:
                    return TypeAdder(type, HappyType.Number);
                case UnaryOperatorType.Plus:
                    return TypeAdder(type, HappyType.Number);
                default:
                    throw new FormatException("Uknown Unary Operator");
            }
        }

        private TypeDescriptor TypeAdder(HappyType type1, HappyType type2)
        {
            if (type1 == type2)
                return new TypeDescriptor(type1);
            else
                return new TypeDescriptor(HappyType.Error);
        }

        public override string ToString()
        {
            return Operator.ToString() + Expression.ToString(); //Todo, ensure that expression and operator impl a ToString
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            
        }
    }
}