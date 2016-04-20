using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Runtime.InteropServices;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class InterpretationVisitor : InterpreterState, IVisitor<Value, VisitorArgs>
    {
        private readonly IInteractiveShell _shell;
        public EventReporter Reporter { get; }
        private CallFrame CurrentCallFrame => _callStack.Peek();
        private Stack<CallFrame> _callStack;

        public InterpretationVisitor(EventReporter reporter, IInteractiveShell shell)
        {
            _shell = shell;
            Reporter = reporter;
            _callStack = new Stack<CallFrame>();
        }

        public Value Visit(ActualParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            var expressionNode = node.Expressions.First();
            var value = expressionNode.Accept(this, arg);
            if (node.Identifiers.Count() == 1)
            {
                var identifierNode = node.Identifiers.First();

                if (value is ReturnValue)
                {
                    var returnValue = value as ReturnValue;
                    var firstReturnValue = returnValue.ReturnValues.First();
                    CurrentCallFrame.Set(identifierNode.Name, firstReturnValue);
                }
                else
                {
                    CurrentCallFrame.Set(identifierNode.Name, value);
                }
            }
            else
            {
                var returnValues = value as ReturnValue;

                for (int i = 0; i < node.Identifiers.Count; i++)
                {
                    var returnValue = returnValues.ReturnValues[i];
                    var identifier = node.Identifiers[i];
                    CurrentCallFrame.Set(identifier.Name, returnValue);
                }
            }
            return null;
        }

        public Value Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            var left = node.LeftOperand.Accept(this, arg);
            var right = node.RightOperand.Accept(this, arg);

            switch (node.Operator)
            {
                case BinaryOperatorType.Plus:
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
                case BinaryOperatorType.Minus:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new NumberValue(leftNumb.Number - rightNumb.Number);
                    }
                case BinaryOperatorType.Times:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new NumberValue(leftNumb.Number * rightNumb.Number);
                    }
                case BinaryOperatorType.Division:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new NumberValue(leftNumb.Number / rightNumb.Number);
                    }
                case BinaryOperatorType.Modulo:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new NumberValue(leftNumb.Number % rightNumb.Number);
                    }
                case BinaryOperatorType.Equals:
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
                case BinaryOperatorType.GreaterThan:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new BooleanValue(leftNumb.Number > rightNumb.Number);
                    }
                case BinaryOperatorType.GreaterOrEqual:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new BooleanValue(leftNumb.Number >= rightNumb.Number);
                    }
                case BinaryOperatorType.LessThan:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new BooleanValue(leftNumb.Number < rightNumb.Number);
                    }
                case BinaryOperatorType.LessOrEqual:
                    {
                        var leftNumb = left as NumberValue;
                        var rightNumb = right as NumberValue;
                        return new BooleanValue(leftNumb.Number <= rightNumb.Number);
                    }
                case BinaryOperatorType.Or:
                    {
                        var leftBool = left as BooleanValue;
                        var rightBool = right as BooleanValue;
                        return new BooleanValue(leftBool.Boolean || rightBool.Boolean);
                    }
                case BinaryOperatorType.And:
                    {
                        var leftBool = left as BooleanValue;
                        var rightBool = right as BooleanValue;
                        return new BooleanValue(leftBool.Boolean && rightBool.Boolean);
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

        public Value Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                CurrentCallFrame.Allocate(identifier.Name);
                Value value = node.Expressions.First().Accept(this, arg);
                CurrentCallFrame.Set(identifier.Name, value);
            }
            return null;
        }

        public Value Visit(DeclStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                CurrentCallFrame.Allocate(identifier.Name);
                Value defaultValue = new UndefinedValue();
                switch (node.Type)
                {
                    case HappyType.Number:
                        defaultValue = new NumberValue(0);
                        break;
                    case HappyType.Text:
                        defaultValue = new TextValue(string.Empty);
                        break;
                    case HappyType.Boolean:
                        defaultValue = new BooleanValue(false);
                        break;
                }
                CurrentCallFrame.Set(identifier.Name, defaultValue);
            }
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

            CallFrame newFrame = new CallFrame(node.DeclarationNode);
            for (int i = 0; i < node.DeclarationNode.Parameters.Count; i++)
            {
                var actualParam = node.Parameters[i];
                var formalParam = node.DeclarationNode.Parameters[i];
                var value = actualParam.Accept(this, arg);

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
            return CurrentCallFrame.Get<Value>(node.Name);
        }

        public Value Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            var value = node.Predicate.Accept(this, arg) as BooleanValue;
            if (value.Boolean)
            {
                node.Body.Accept(this, arg);
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
                node.Body.Accept(this, arg);
            }
            else
            {
                node.ElseIfStatements.Accept(this, arg);
            }

            return null;
        }

        public Value Visit(LiteralValueNode node, VisitorArgs arg)
        {
            switch (node.Type)
            {
                case HappyType.Number:
                    return new NumberValue(double.Parse(node.Value));
                case HappyType.Text:
                    return new TextValue(node.Value);
                case HappyType.Boolean:
                    return new BooleanValue("true".Equals(node.Value, StringComparison.InvariantCultureIgnoreCase));
                default:
                    return new UndefinedValue();
            }
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
    }
}
