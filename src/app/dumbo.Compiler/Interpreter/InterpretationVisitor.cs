using System;
using System.Collections.Generic;
using System.Globalization;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class InterpretationVisitor : InterpreterState, IVisitor<Value, VisitorArgs>
    {
        private readonly IInteractiveShell _shell;
        public EventReporter Reporter { get; }
        private CallFrame CurrentCallFrame => _callStack.Peek();
        private readonly Stack<CallFrame> _callStack;
        private readonly Dictionary<string, Value> _constants;
        
        private IFormatProvider EnglishCulture = new CultureInfo("en");

        public InterpretationVisitor(EventReporter reporter, IInteractiveShell shell)
        {
            _shell = shell;
            Reporter = reporter;
            _callStack = new Stack<CallFrame>();
            _constants = new Dictionary<string, Value>();
        }

        public Value Visit(ActualParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(ArrayDeclStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                CurrentCallFrame.Allocate(identifier.Name);
                var arrayValue = new ArrayValue(node.Type.Type.Type, GetArraySizes(node.Type.Sizes, arg));
                CurrentCallFrame.Set(identifier.Name, arrayValue);
            }

            return null;
        }

        public Value Visit(ArrayIdentifierNode node, VisitorArgs arg)
        {
            var array = CurrentCallFrame.Get<ArrayValue>(node.Name);
            var sizes = GetArraySizes(node.Indices, arg);
            var value = array.GetValue(sizes);
            return value;
        }

        public Value Visit(ArrayTypeNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public Value Visit(ArrayValueNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public Value Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            PerformAssignment(node.Identifiers, node.Value, arg);
            return null;
        }

        public Value Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            switch (node.Operator)
            {
                case BinaryOperatorType.Plus:
                    {
                        var left = node.LeftOperand.Accept(this, arg);
                        var right = node.RightOperand.Accept(this, arg);

                        if (left is NumberValue && right is TextValue)
                        {
                            var leftNumb = left as NumberValue;
                            var rightText = right as TextValue;
                            return new TextValue(leftNumb.Number + rightText.Text);
                        }
                        else if (left is TextValue && right is NumberValue)
                        {
                            var leftText = left as TextValue;
                            var rightNumb = right as NumberValue;
                            return new TextValue(leftText.Text + rightNumb.Number);
                        }
                        else if (left is TextValue && right is BooleanValue)
                        {
                            var leftText = left as TextValue;
                            var rightNumb = right as BooleanValue;
                            return new TextValue(leftText.Text + rightNumb.Boolean);
                        }
                        else if (left is BooleanValue && right is TextValue)
                        {
                            var leftText = left as BooleanValue;
                            var rightNumb = right as TextValue;
                            return new TextValue(leftText.Boolean + rightNumb.Text);
                        }
                        else if (left is TextValue)
                        {
                            var leftText = left as TextValue;
                            var rightText = right as TextValue;
                            return new TextValue(leftText.Text + rightText.Text);
                        }
                        else if (left is NumberValue)
                        {
                            var leftNumb = left as NumberValue;
                            var rightNumb = right as NumberValue;
                            return new NumberValue(leftNumb.Number + rightNumb.Number);
                        }
                        break;
                    }
                case BinaryOperatorType.Minus:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new NumberValue(left.Number - right.Number);
                    }
                case BinaryOperatorType.Times:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new NumberValue(left.Number * right.Number);
                    }
                case BinaryOperatorType.Division:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        if (right.Number == 0)
                            return new NumberValue(double.NaN);
                        else
                            return new NumberValue(left.Number / right.Number);
                    }
                case BinaryOperatorType.Modulo:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new NumberValue(left.Number % right.Number);
                    }
                case BinaryOperatorType.Equals:
                    {
                        var left = node.LeftOperand.Accept(this, arg);
                        var right = node.RightOperand.Accept(this, arg);
                        if (left is NumberValue)
                        {
                            var leftNumb = left as NumberValue;
                            var rightNumb = right as NumberValue;
                            return new BooleanValue(leftNumb.Number == rightNumb.Number);
                        }
                        else if (left is BooleanValue)
                        {
                            var leftBool = left as BooleanValue;
                            var rightBool = right as BooleanValue;
                            return new BooleanValue(leftBool.Boolean == rightBool.Boolean);
                        }
                        else if (left is TextValue)
                        {
                            var leftText = left as TextValue;
                            var rightText = right as TextValue;
                            return new BooleanValue(leftText.Text == rightText.Text);
                        }
                        break;
                    }
                case BinaryOperatorType.GreaterThan:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new BooleanValue(left.Number > right.Number);
                    }
                case BinaryOperatorType.GreaterOrEqual:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new BooleanValue(left.Number >= right.Number);
                    }
                case BinaryOperatorType.LessThan:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new BooleanValue(left.Number < right.Number);
                    }
                case BinaryOperatorType.LessOrEqual:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as NumberValue;
                        var right = node.RightOperand.Accept(this, arg) as NumberValue;
                        return new BooleanValue(left.Number <= right.Number);
                    }
                case BinaryOperatorType.Or:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as BooleanValue;
                        if (left.Boolean == true)
                            return left;
                        var right = node.RightOperand.Accept(this, arg) as BooleanValue;
                        return right;
                    }
                case BinaryOperatorType.And:
                    {
                        var left = node.LeftOperand.Accept(this, arg) as BooleanValue;
                        if (left.Boolean == false)
                            return left;
                        var right = node.RightOperand.Accept(this, arg) as BooleanValue;
                        return right;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        public Value Visit(BreakStmtNode node, VisitorArgs arg)
        {
            throw new BreakException();
        }

        public Value Visit(ContinueStmtNode node, VisitorArgs arg)
        {
            throw new ContinueException();
        }

        public Value Visit(ConstDeclListNode node, VisitorArgs arg)
        {
            foreach (var constant in node)
                constant.Accept(this, arg);
            return null;
        }

        public Value Visit(ConstDeclNode node, VisitorArgs arg)
        {
            var val = node.Value.Accept(this, arg);
            _constants.Add(node.Name, val);

            return null;
        }

        public Value Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
                CurrentCallFrame.Allocate(identifier.Name);

            PerformAssignment(node.Identifiers, node.Value, arg);
            return null;
        }

        public Value Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                var val = item.Accept(this, arg) as BooleanValue;
                if (val.Boolean)
                {
                    return val;
                }
            }

            return new BooleanValue(false);
        }

        public Value Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            var value = node.Predicate.Accept(this, arg) as BooleanValue;
            if (value.Boolean)
            {
                node.Body.Accept(this, arg);
            }

            return value;
        }

        public Value Visit(ExpressionListNode node, VisitorArgs arg)
        {
            // TODO Refactor nodes that use expression list
            throw new System.NotImplementedException();
        }

        public Value Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(FormalParamNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            if (IsBuiltIn(node.DeclarationNode))
                return CallBuiltInFunction(node, arg);

            List<Value> valueList = GetActualParameterValues(node, arg);

            CallFrame newFrame = new CallFrame(node.DeclarationNode);
            for (int i = 0; i < node.DeclarationNode.Parameters.Count; i++)
            {
                var formalParam = node.DeclarationNode.Parameters[i];
                var value = valueList[i];

                newFrame.Allocate(formalParam.Name);
                newFrame.Set(formalParam.Name, value);
            }

            _callStack.Push(newFrame);

            Value val = null;

            try
            {
                node.DeclarationNode.Body.Accept(this, arg);
            }
            catch (ReturnFromFunctionException e)
            {
                if (e.ReturnValue.ReturnValues.Count == 1)
                {
                    val = e.ReturnValue.ReturnValues[0];
                }
                else
                {
                    val = e.ReturnValue;
                }
            }

            _callStack.Pop();

            return val;
        }

        public Value Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);
            return null;
        }

        public Value Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var func in node)
            {
                func.Accept(this, arg);
            }
            return null;
        }

        public Value Visit(FuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(IdentifierListNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(IdentifierNode node, VisitorArgs arg)
        {
            if (_constants.ContainsKey(node.Name))
                return _constants[node.Name];

            return CurrentCallFrame.Get<Value>(node.Name);
        }

        public Value Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            var value = node.Predicate.Accept(this, arg) as BooleanValue;
            if (value.Boolean)
            {
                node.Then.Accept(this, arg);
            }
            else
            {
                var elseIfVal = node.ElseIfStatements.Accept(this, arg) as BooleanValue;
                if (!elseIfVal.Boolean)
                {
                    node.Else.Accept(this, arg);
                }
            }
            return null;
        }

        public Value Visit(IfStmtNode node, VisitorArgs arg)
        {
            var value = node.Predicate.Accept(this, arg) as BooleanValue;
            if (value.Boolean)
            {
                node.Then.Accept(this, arg);
            }
            else
            {
                node.ElseIfStatements.Accept(this, arg);
            }

            return null;
        }

        public Value Visit(LiteralValueNode node, VisitorArgs arg)
        {
            var primitiveTypeNode = node.Type as PrimitiveTypeNode;
            if (primitiveTypeNode != null)
            {
                switch (primitiveTypeNode.Type)
                {
                    case PrimitiveType.Number:
                        return new NumberValue(double.Parse(node.Value, EnglishCulture));
                    case PrimitiveType.Text:
                        return new TextValue(node.Value);
                    case PrimitiveType.Boolean:
                        return new BooleanValue("true".Equals(node.Value, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            throw new InvalidOperationException("Unknown type");
        }

        public Value Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                CurrentCallFrame.Allocate(identifier.Name);
                Value defaultValue = new UndefinedValue();
                var typeNode = node.Type as PrimitiveTypeNode;
                switch (typeNode.Type)
                {
                    case PrimitiveType.Number:
                        defaultValue = new NumberValue(0);
                        break;
                    case PrimitiveType.Text:
                        defaultValue = new TextValue(string.Empty);
                        break;
                    case PrimitiveType.Boolean:
                        defaultValue = new BooleanValue(false);
                        break;
                }
                CurrentCallFrame.Set(identifier.Name, defaultValue);
            }
            return null;
        }

        public Value Visit(ProgramNode node, VisitorArgs arg)
        {
            _callStack.Push(new CallFrame(node));
            try
            {
                node.Body.Accept(this, arg);
            }
            catch (InterpretationErrorException e)
            {
                Reporter.Error(e.Message, e.Node.SourcePosition);
            }
            _callStack.Pop();
            return null;
        }

        public Value Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            var accept = node.Number.Accept(this, arg) as NumberValue;
            double currentValue = 1;
            double toValue = accept.Number;

            while (true)
            {
                if (currentValue > toValue || currentValue < 0)
                {
                    break;
                }

                currentValue++;

                try
                {
                    node.Body.Accept(this, arg);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {

                }
            }

            return null;
        }

        public Value Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            var predicateVal = node.Predicate.Accept(this, arg) as BooleanValue;
            while (predicateVal.Boolean)
            {
                try
                {
                    node.Body.Accept(this, arg);
                }
                catch (BreakException)
                {
                    break;
                }
                predicateVal = node.Predicate.Accept(this, arg) as BooleanValue;
            }

            return null;
        }

        public Value Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            if (_callStack.Count == 0)
            {
                Reporter.Error("Wrong return statement call.", new SourcePosition(0, 0, 0, 0));
            }
            else
            {
                var returnValue = new ReturnValue();
                foreach (var expression in node.Expressions)
                {
                    var value = expression.Accept(this, arg);
                    returnValue.ReturnValues.Add(value);

                }
                throw new ReturnFromFunctionException(returnValue);
            }

            return null;
        }

        public Value Visit(RootNode node, VisitorArgs arg)
        {
            node.ConstDecls.Accept(this, arg);
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, arg);
            return null;
        }

        public Value Visit(StmtBlockNode node, VisitorArgs arg)
        {
            CurrentCallFrame.EnterBlock();

            foreach (var stmt in node)
            {
                stmt.Accept(this, arg);
            }

            CurrentCallFrame.ExitBlock();
            return null;
        }

        public Value Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            var operand = node.Expression.Accept(this, arg);

            switch (node.Operator)
            {
                case UnaryOperatorType.Not:
                    {
                        var operandBool = operand as BooleanValue;
                        return new BooleanValue(!operandBool.Boolean);
                    }
                case UnaryOperatorType.Minus:
                    {
                        var operandNumb = operand as NumberValue;
                        return new NumberValue(-operandNumb.Number);
                    }
                case UnaryOperatorType.Plus:
                    {
                        var operandNumb = operand as NumberValue;
                        return new NumberValue(+operandNumb.Number);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsBuiltIn(FuncDeclNode declarationNode)
        {
            return declarationNode is BuiltInFuncDeclNode;
        }

        private Value CallBuiltInFunction(FuncCallExprNode node, VisitorArgs arg)
        {
            var builtIn = node.DeclarationNode as BuiltInFuncDeclNode;

            switch (builtIn.Type)
            {
                case BuiltInFunction.Write:
                    var text = node.Parameters[0].Accept(this, arg) as TextValue;
                    _shell.Write(text.Text);
                    return null;
                case BuiltInFunction.ReadNumber:
                    return _shell.ReadNumber();
                case BuiltInFunction.ReadText:
                    return _shell.ReadText();
                case BuiltInFunction.Random:
                    var minValue = node.Parameters[0].Accept(this, arg) as NumberValue;
                    var maxValue = node.Parameters[1].Accept(this, arg) as NumberValue;
                    var min = (int)minValue.Number;
                    var max = (int)maxValue.Number;
                    if (min < max)
                    {
                        var randomNumber = new Random().Next(min, max);
                        return new NumberValue(randomNumber);
                    }
                    else
                    {
                        throw new InterpretationErrorException("Maximum value less than the minimum value", node);
                    }
                case BuiltInFunction.Floor:
                    var floorValue = node.Parameters[0].Accept(this, arg) as NumberValue;
                    var floor = (double)floorValue.Number;
                    return new NumberValue(Math.Floor(floor));
                case BuiltInFunction.Ceiling:
                    var ceilingValue = node.Parameters[0].Accept(this, arg) as NumberValue;
                    var ceiling = (double)ceilingValue.Number;
                    return new NumberValue(Math.Ceiling(ceiling));

                case BuiltInFunction.IsEqual:
                    var v1 = node.Parameters[0].Accept(this, arg) as TextValue;
                    var v2 = node.Parameters[1].Accept(this, arg) as TextValue;
                    return new BooleanValue(v1.Text.Equals(v2.Text, StringComparison.CurrentCultureIgnoreCase));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PerformAssignment(IdentifierListNode identifiers, ExpressionNode expression, VisitorArgs arg)
        {
            var value = expression.Accept(this, arg);

            if (identifiers.Count == 1)
            {
                if (identifiers[0] is ArrayIdentifierNode)
                {
                    var id = identifiers[0] as ArrayIdentifierNode;
                    var arrayValue = CurrentCallFrame.Get<ArrayValue>(id.Name);
                    arrayValue.SetValue(GetArraySizes(id.Indices, arg), value);
                }
                else
                {
                    CurrentCallFrame.Set(identifiers[0].Name, value);
                }
            }
            else
            {
                var multiVal = value as ReturnValue;
                for (int i = 0; i < identifiers.Count; i++)
                {
                    var identifier = identifiers[i];
                    var val = multiVal.ReturnValues[i];

                    CurrentCallFrame.Set(identifier.Name, val);
                }
            }
        }

        private List<int> GetArraySizes(ExpressionListNode list, VisitorArgs arg)
        {
            List<int> sizeList = new List<int>();
            foreach (var size in list)
            {
                var sizeVal = size.Accept(this, arg) as NumberValue;
                sizeList.Add((int)Math.Floor(sizeVal.Number));
            }


            return sizeList;
        }

        private List<Value> GetActualParameterValues(FuncCallExprNode node, VisitorArgs arg)
        {
            var valueList = new List<Value>();
            foreach (var actualParam in node.Parameters)
            {
                var value = actualParam.Accept(this, arg);
                if (value is ReturnValue)
                {
                    var retVal = value as ReturnValue;
                    valueList.AddRange(retVal.ReturnValues);
                }
                else
                {
                    valueList.Add(value);
                }
            }

            return valueList;
        }
    }
}
