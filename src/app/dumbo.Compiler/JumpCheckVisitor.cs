using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    class JumpCheckVisitor : IVisitor<VisitResult, VisitorArgs>
    {
        public EventReporter Reporter { get; }

        private Stack<bool> InLoops { get; set; }

        public JumpCheckVisitor(EventReporter reporter)
        {
            Reporter = reporter;
        }

        public VisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            if(InLoops.Count == 0)
                Reporter.Error("Break stmt is not placed in a loop.", node.SourcePosition);
            return null;
        }

        public VisitResult Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(DeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            foreach (var stmt in node.Body)
            {
                if(stmt is BreakStmtNode)
                    Reporter.Error("There cannot be a Break statement in the program body.", node.SourcePosition);
                stmt.Accept(this, arg);
            }
            return null;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            InLoops.Push(true);
            node.Body.Accept(this, arg);
            InLoops.Pop();
            return null;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            InLoops.Push(true);
            node.Body.Accept(this, arg);
            InLoops.Pop();
            return null;
        }

        public VisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }

        public VisitResult Visit(RootNode node, VisitorArgs arg)
        {
            node.Program.Accept(this, arg);
            node.FuncDecls.Accept(this, arg);
            return null;
        }

        public VisitResult Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var stmt in node)
            {
                stmt.Accept(this, arg);
            }
            return null;
        }

        public VisitResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            throw new NotImplementedException();
        }
    }
}
