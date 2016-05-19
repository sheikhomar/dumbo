using System;
using System.Collections.Generic;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.TypeChecking
{
    public class BinaryOperationTypeChecker
    {
        private class Rule
        {
            public Rule(PrimitiveType operandType1, PrimitiveType operandType2, PrimitiveType resultType)
            {
                OperandType1 = operandType1;
                OperandType2 = operandType2;
                ResultType = resultType;
            }

            public PrimitiveType OperandType1 { get; }
            public PrimitiveType OperandType2 { get; }
            public PrimitiveType ResultType { get; }
        }

        private readonly Dictionary<BinaryOperatorType, IList<Rule>> _container;

        public BinaryOperationTypeChecker()
        {
            _container = new Dictionary<BinaryOperatorType, IList<Rule>>();
            Initialise();
        }

        private void Initialise()
        {
            // Rules for arithmetic expressions
            Add(BinaryOperatorType.Plus, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Number);
            Add(BinaryOperatorType.Plus, PrimitiveType.Text, PrimitiveType.Text, PrimitiveType.Text);
            Add(BinaryOperatorType.Minus, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Number);
            Add(BinaryOperatorType.Times, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Number);
            Add(BinaryOperatorType.Division, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Number);
            Add(BinaryOperatorType.Modulo, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Number);

            // Rules for relational expressions
            Add(BinaryOperatorType.Equals, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Boolean);
            Add(BinaryOperatorType.Equals, PrimitiveType.Boolean, PrimitiveType.Boolean, PrimitiveType.Boolean);
            Add(BinaryOperatorType.Equals, PrimitiveType.Text, PrimitiveType.Text, PrimitiveType.Boolean);
            Add(BinaryOperatorType.GreaterThan, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Boolean);
            Add(BinaryOperatorType.GreaterOrEqual, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Boolean);
            Add(BinaryOperatorType.LessThan, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Boolean);
            Add(BinaryOperatorType.LessOrEqual, PrimitiveType.Number, PrimitiveType.Number, PrimitiveType.Boolean);

            // Rules for logical expressions
            Add(BinaryOperatorType.Or, PrimitiveType.Boolean, PrimitiveType.Boolean, PrimitiveType.Boolean);
            Add(BinaryOperatorType.And, PrimitiveType.Boolean, PrimitiveType.Boolean, PrimitiveType.Boolean);
        }

        private void Add(BinaryOperatorType operationType, PrimitiveType operandType1,
            PrimitiveType operandType2, PrimitiveType resultType)
        {
            if (!_container.ContainsKey(operationType))
            {
                _container.Add(operationType, new List<Rule>());
            }

            _container[operationType].Add(new Rule(operandType1, operandType2, resultType));
        }

        public Tuple<bool, PrimitiveType> GetInferredType(BinaryOperatorType operationType,
            TypeNode leftNode,
            TypeNode rightNode)
        {
            PrimitiveType leftOperand;
            PrimitiveType rightOperand;

            if(leftNode is PrimitiveTypeNode)
                leftOperand = (leftNode as PrimitiveTypeNode).Type;
            else if (leftNode is ArrayTypeNode)
                leftOperand = (leftNode as ArrayTypeNode).Type.Type;
            else
                return new Tuple<bool, PrimitiveType>(false, default(PrimitiveType));

            if (rightNode is PrimitiveTypeNode)
                rightOperand = (rightNode as PrimitiveTypeNode).Type;
            else if (rightNode is ArrayTypeNode)
                rightOperand = (rightNode as ArrayTypeNode).Type.Type;
            else
                return new Tuple<bool, PrimitiveType>(false, default(PrimitiveType));

            if (_container.ContainsKey(operationType))
            {
                var specList = _container[operationType];
                foreach (var spec in specList)
                {
                    if ((leftOperand == spec.OperandType1 && rightOperand == spec.OperandType2) ||
                        (leftOperand == spec.OperandType2 && rightOperand == spec.OperandType1))
                    {
                        return new Tuple<bool, PrimitiveType>(true, spec.ResultType);
                    }
                }
            }

            return new Tuple<bool, PrimitiveType>(false, default(PrimitiveType));
        }
    }
}
