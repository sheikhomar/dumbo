using System;
using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class InterpretationVisitor : InterpreterState, IVisitor<Value, VisitorArgs>
    {
        public EventReporter Reporter { get; }

        private Dictionary<string, KnownAddress> identifierDictionary;

        public InterpretationVisitor(EventReporter reporter)
        {
            Reporter = reporter;
            identifierDictionary = new Dictionary<string, KnownAddress>();
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
                _data[knownAddress.Address] = value;
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
                _data.Add(new UndefinedValue());
                identifierDictionary.Add(identifier.Name, new KnownAddress(_data.Count - 1));
            }
            // TODO Copy assignment code
            return null;
        }

        public Value Visit(DeclStmtNode node, VisitorArgs arg)
        {
            foreach (var identifier in node.Identifiers)
            {
                _data.Add(new UndefinedValue());
                identifierDictionary.Add(identifier.Name, new KnownAddress(_data.Count - 1));
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
            throw new System.NotImplementedException();
        }

        public Value Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
        }

        public Value Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            return null;
        }

        public Value Visit(FuncDeclNode node, VisitorArgs arg)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public Value Visit(RootNode node, VisitorArgs arg)
        {
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, arg);
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
    }
}