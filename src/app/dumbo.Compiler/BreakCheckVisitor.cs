using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class BreakCheckVisitor : IVisitor<VisitResult, VisitorArgs>
    {
        public EventReporter Reporter { get; }

        private int InLoops { get; set; }

        public BreakCheckVisitor(EventReporter reporter)
        {
            Reporter = reporter;
        }

        public VisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(ArrayDeclStmtNode node, VisitorArgs arg)
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
            if(InLoops == 0)
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

        public VisitResult Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }
            return null;
        }

        public VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);
            return null;
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
            node.DeclarationNode.Accept(this, arg);
            return null;
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.DeclarationNode.Accept(this, arg);
            return null;
        }

        public VisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var funcDecl in node)
            {
                funcDecl.Accept(this, arg);
            }

            return null;
        }

        public VisitResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);
            return null;
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
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            node.Else.Accept(this, arg);
            return null;
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            return null;
        }

        public VisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            return null;
        }

        public VisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);
            return null;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            InLoops++;
            node.Body.Accept(this, arg);
            InLoops--;
            return null;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            InLoops++;
            node.Body.Accept(this, arg);
            InLoops--;
            return null;
        }

        public VisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            return null;
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
            return null;
        }
    }
}
