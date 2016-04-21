using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    class ReachabilityVisitor : IVisitor<ReachabilityResult, ReachabilityArgument>
    {
        public EventReporter Reporter { get; }

        public ReachabilityVisitor(EventReporter reporter)
        {
            Reporter = reporter;
        }

        public ReachabilityResult Visit(ActualParamListNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(AssignmentStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(BinaryOperationNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(BreakStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(BuiltInFuncDeclNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(DeclAndAssignmentStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(DeclStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(ElseIfStmtListNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(ElseIfStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(ExpressionListNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(FormalParamListNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(FormalParamNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(FuncCallExprNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(FuncCallStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(FuncDeclListNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(FuncDeclNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(IdentifierListNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(IdentifierNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(IfElseStmtNode node, ReachabilityArgument arg)
        {
            var ifPart = node.Body.Accept(this, arg).TerminatesNormally;
            var elsePart = node.Else.Accept(this, arg).TerminatesNormally;

            return new ReachabilityResult(ifPart || elsePart);
        }

        public ReachabilityResult Visit(IfStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(LiteralValueNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(ProgramNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(RepeatStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(RepeatWhileStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(ReturnStmtNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(RootNode node, ReachabilityArgument arg)
        {
            var FuncsTerminatesNormally = node.FuncDecls.Accept(this, arg).TerminatesNormally;
            var ProgramTerminatesNormally = node.Program.Accept(this, arg).TerminatesNormally;

            return null;
        }

        public ReachabilityResult Visit(StmtBlockNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }

        public ReachabilityResult Visit(UnaryOperationNode node, ReachabilityArgument arg)
        {
            throw new NotImplementedException();
        }
    }
}
