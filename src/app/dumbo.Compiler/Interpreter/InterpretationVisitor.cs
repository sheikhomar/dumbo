using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class InterpretationVisitor : InterpreterState, IVisitor<Value, VisitorArgs>
    {
        public EventReporter Reporter { get; }
        public CallFrame CurrentCallFrame => callStack.Peek();

        //private Dictionary<string, KnownAddress> identifierDictionary;

        private Stack<CallFrame> callStack;

        public InterpretationVisitor(EventReporter reporter)
        {
            Reporter = reporter;
            //identifierDictionary = new Dictionary<string, KnownAddress>();
            callStack = new Stack<CallFrame>();
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
                    CurrentCallFrame.CurrentBlockFrame.Set(identifierNode.Name, firstReturnValue);
                }
                else
                {
                    CurrentCallFrame.CurrentBlockFrame.Set(identifierNode.Name, value);
                }
            }
            else
            {
                var returnValues = value as ReturnValue;

                for (int i = 0; i < node.Identifiers.Count; i++)
                {
                    var returnValue = returnValues.ReturnValues[i];
                    var identifier = node.Identifiers[i];
                    CurrentCallFrame.CurrentBlockFrame.Set(identifier.Name, returnValue);
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
            throw new System.NotImplementedException();
        }

        public Value Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                CurrentCallFrame.CurrentBlockFrame.Allocate(identifier.Name);
                Value value = node.Expressions.First().Accept(this, arg);
                CurrentCallFrame.CurrentBlockFrame.Set(identifier.Name, value);
            }
            return null;
        }

        public Value Visit(DeclStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                CurrentCallFrame.CurrentBlockFrame.Allocate(identifier.Name);
            }
            return null;
        }

        public Value Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(ExpressionListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(FormalParamListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(FormalParamNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            CallFrame frame = new CallFrame(node.DeclarationNode);
            callStack.Push(frame);

            for (int i = 0; i < node.DeclarationNode.Parameters.Count; i++)
            {
                var actualParam = node.Parameters[i];
                var formalParam = node.DeclarationNode.Parameters[i];
                var value = actualParam.Accept(this, arg);

                CurrentCallFrame.CurrentBlockFrame.Allocate(formalParam.Name);
                CurrentCallFrame.CurrentBlockFrame.Set(formalParam.Name, value);
            }

            var val = new ReturnValue();

            try
            {
                node.DeclarationNode.Body.Accept(this, arg);
            }
            catch (ReturnFromFunctionException e)
            {
                val = e.ReturnValue;
            }

            callStack.Pop();

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
            //foreach (var parameter in node.Parameters)
            //{
            //    AllocateMemory(parameter.Name, new UndefinedValue());
            //}

            //var values = new ReturnValue();
            //foreach (var returnType in node.ReturnTypes)
            //{
            //    values.ReturnValues.Add(new UndefinedValue());
            //}
            //AllocateMemory(node.Name, values);
            return null;
        }

        public Value Visit(IdentifierListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(IdentifierNode node, VisitorArgs arg)
        {
            //return identifierDictionary[node.Name];
            // TODO Shall return value
            return null;
        }

        public Value Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(IfStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
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
            callStack.Push(new CallFrame(node));
            node.Body.Accept(this, arg);
            return null;
        }

        public Value Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            if (callStack.Count == 0)
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
                //var callFrame = callStack.Pop();
                //var funcDeclNode = callFrame.Function;
                //var knownAddress = identifierDictionary[funcDeclNode.Name];
                //var returnValue = _data[knownAddress.Address] as ReturnValue;
                //for (int i = 0; i < node.Expressions.Count; i++)
                //{
                //    returnValue.ReturnValues[i] = node.Expressions[i].Accept(this, arg);
                //}
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

        private void AllocateMemory(string name, Value value)
        {
            var latestCallFrame = callStack.Peek();
            latestCallFrame.CurrentBlockFrame.Allocate(name);
        }
    }
}
