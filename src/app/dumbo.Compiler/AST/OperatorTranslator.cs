using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.AST
{
    public static class OperatorTranslator
    {
        public static string BinaryOperatorTypeTranslator(BinaryOperatorType input)
        {
            switch (input)
            {
                case BinaryOperatorType.And:
                    return "AND";
                case BinaryOperatorType.Division:
                    return "/";
                case BinaryOperatorType.Equals:
                    return "=";
                case BinaryOperatorType.GreaterOrEqual:
                    return ">=";
                case BinaryOperatorType.GreaterThan:
                    return ">";
                case BinaryOperatorType.LessOrEqual:
                    return "<=";
                case BinaryOperatorType.LessThan:
                    return "<";
                case BinaryOperatorType.Minus:
                    return "-";
                case BinaryOperatorType.Modulo:
                    return "%";
                case BinaryOperatorType.Or:
                    return "OR";
                case BinaryOperatorType.Plus:
                    return "+";
                case BinaryOperatorType.Times:
                    return "*";
                default:
                    return "__OPR_ERROR__";
            }
        }

        static public string UnaryOperatorTypeTranslator(UnaryOperatorType input)
        {
            switch (input)
            {
                case UnaryOperatorType.Minus:
                    return "-";
                case UnaryOperatorType.Not:
                    return "NOT";
                case UnaryOperatorType.Plus:
                    return "+";
                default:
                    return "__OPR_ERROR__";
            }
        }
    }
}
