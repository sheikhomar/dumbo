using System.Collections.Generic;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class BinaryOperationTypeChecker
    {
        private class Rule
        {
            public Rule(HappyType operandType1, HappyType operandType2, HappyType resultType)
            {
                OperandType1 = operandType1;
                OperandType2 = operandType2;
                ResultType = resultType;
            }

            public HappyType OperandType1 { get; }
            public HappyType OperandType2 { get; }
            public HappyType ResultType { get; }
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
            Add(BinaryOperatorType.Plus, HappyType.Number, HappyType.Number, HappyType.Number);
            Add(BinaryOperatorType.Plus, HappyType.Text, HappyType.Text, HappyType.Text);
            Add(BinaryOperatorType.Plus, HappyType.Text, HappyType.Number, HappyType.Text);
            Add(BinaryOperatorType.Minus, HappyType.Number, HappyType.Number, HappyType.Number);
            Add(BinaryOperatorType.Times, HappyType.Number, HappyType.Number, HappyType.Number);
            Add(BinaryOperatorType.Division, HappyType.Number, HappyType.Number, HappyType.Number);
            Add(BinaryOperatorType.Modulo, HappyType.Number, HappyType.Number, HappyType.Number);

            // Rules for relational expressions
            Add(BinaryOperatorType.Equals, HappyType.Number, HappyType.Number, HappyType.Boolean);
            Add(BinaryOperatorType.GreaterThan, HappyType.Number, HappyType.Number, HappyType.Boolean);
            Add(BinaryOperatorType.GreaterOrEqual, HappyType.Number, HappyType.Number, HappyType.Boolean);
            Add(BinaryOperatorType.LessThan, HappyType.Number, HappyType.Number, HappyType.Boolean);
            Add(BinaryOperatorType.LessOrEqual, HappyType.Number, HappyType.Number, HappyType.Boolean);

            // Rules for logical expressions
            Add(BinaryOperatorType.Or, HappyType.Boolean, HappyType.Boolean, HappyType.Boolean);
            Add(BinaryOperatorType.And, HappyType.Boolean, HappyType.Boolean, HappyType.Boolean);
        }

        private void Add(BinaryOperatorType operationType, HappyType operandType1, 
            HappyType operandType2, HappyType resultType)
        {
            if (!_container.ContainsKey(operationType))
            {
                _container.Add(operationType, new List<Rule>());
            }

            _container[operationType].Add(new Rule(operandType1, operandType2, resultType));
        }

        public HappyType GetInferredType(BinaryOperatorType operationType, HappyType leftOperand,
            HappyType rightOperand)
        {
            if (_container.ContainsKey(operationType))
            {
                var specList = _container[operationType];
                foreach (var spec in specList)
                {
                    if ((leftOperand == spec.OperandType1 && rightOperand == spec.OperandType2) ||
                        (leftOperand == spec.OperandType2 && rightOperand == spec.OperandType1))
                    {
                        return spec.ResultType;
                    }
                }
            }

            return HappyType.Error;
        }
    }
}