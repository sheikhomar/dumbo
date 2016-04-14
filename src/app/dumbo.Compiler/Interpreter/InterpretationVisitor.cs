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

        private Dictionary<string, KnownAddress> identifierDictionary;

        private Stack<CallFrame> callStack;

        public InterpretationVisitor(EventReporter reporter)
        {
            Reporter = reporter;
            identifierDictionary = new Dictionary<string, KnownAddress>();
            callStack = new Stack<CallFrame>();
        }

        public Value Visit(ActualParamListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            var expressionNode = node.Expressions.First();
            var value = expressionNode.Accept(this, arg);
            if (node.Identifiers.Count() == 1)
            {
                var identifierNode = node.Identifiers.First();
                var knownAddress = identifierNode.Accept(this, arg) as KnownAddress;

                if (value is ReturnValue)
                {
                    var returnValue = value as ReturnValue;
                    _data[knownAddress.Address] = returnValue.ReturnValues.First();
                }
                else
                {
                    _data[knownAddress.Address] = value;
                }
            }
            else
            {
                // TODO Function return
            }
            return null;
        }

        public Value Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(BreakStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                AllocateMemory(identifier.Name, new UndefinedValue());
            }
            // TODO Copy assignment code
            return null;
        }

        public Value Visit(DeclStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                AllocateMemory(identifier.Name, new UndefinedValue());
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
                var formalParam = node.DeclarationNode.Parameters[i];
                var actualParam = node.Parameters[i];
                var knownAddress = identifierDictionary[formalParam.Name];
                _data[knownAddress.Address] = actualParam.Accept(this, arg);
            }

            try
            {
                node.DeclarationNode.Body.Accept(this, arg);
            }
            catch (StopFunctionException)
            {
            }

            return null;
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
            foreach (var parameter in node.Parameters)
            {
                AllocateMemory(parameter.Name, new UndefinedValue());
            }

            var values = new ReturnValue();
            foreach (var returnType in node.ReturnTypes)
            {
                values.ReturnValues.Add(new UndefinedValue());
            }
            AllocateMemory(node.Name, values);
            return null;
        }

        public Value Visit(IdentifierListNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(IdentifierNode node, VisitorArgs arg)
        {
            return identifierDictionary[node.Name];
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
                var callFrame = callStack.Pop();
                var funcDeclNode = callFrame.Function;
                var knownAddress = identifierDictionary[funcDeclNode.Name];
                var returnValue = _data[knownAddress.Address] as ReturnValue;
                for (int i = 0; i < node.Expressions.Count; i++)
                {
                    returnValue.ReturnValues[i] = node.Expressions[i].Accept(this, arg);
                }
                throw new StopFunctionException();
            }

            return null;
        }

        public Value Visit(RootNode node, VisitorArgs arg)
        {
            node.FuncDecls.Accept(this, arg);
            node.Program.Accept(this, arg);
            return null;
        }

        public Value Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var stmt in node)
            {
                stmt.Accept(this, arg);
            }
            return null;
        }

        public Value Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        private void AllocateMemory(string name, Value value)
        {
            var latestCallFrame = callStack.Peek();
            latestCallFrame.Allocate(name, value);
            _data.Add(value);
            identifierDictionary.Add(name, new KnownAddress(_data.Count - 1));
        }
    }
}