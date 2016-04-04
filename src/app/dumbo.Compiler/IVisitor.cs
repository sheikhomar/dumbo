using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public interface IVisitor
    {
        VisitResult Visit(ActualParamListNode node, VisitorArgs arg);
        VisitResult Visit(AssignmentStmtNode node, VisitorArgs arg);
        VisitResult Visit(BinaryOperationNode node, VisitorArgs arg);
        VisitResult Visit(BreakStmtNode node, VisitorArgs arg);
        VisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg);
        VisitResult Visit(DeclStmtNode node, VisitorArgs arg);
        VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg);
        VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg);
        VisitResult Visit(ExpressionListNode node, VisitorArgs arg);
        VisitResult Visit(FormalParamListNode node, VisitorArgs arg);
        VisitResult Visit(FormalParamNode node, VisitorArgs arg);
        VisitResult Visit(FuncCallExprNode node, VisitorArgs arg);
        VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg);
        VisitResult Visit(FuncDeclListNode node, VisitorArgs arg);
        VisitResult Visit(FuncDeclNode node, VisitorArgs arg);
        VisitResult Visit(IdentifierListNode node, VisitorArgs arg);
        VisitResult Visit(IdentifierNode node, VisitorArgs arg);
        VisitResult Visit(IfElseStmtNode node, VisitorArgs arg);
        VisitResult Visit(IfStmtNode node, VisitorArgs arg);
        VisitResult Visit(LiteralValueNode node, VisitorArgs arg);
        VisitResult Visit(ProgramNode node, VisitorArgs arg);
        VisitResult Visit(RepeatStmtNode node, VisitorArgs arg);
        VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg);
        VisitResult Visit(ReturnStmtNode node, VisitorArgs arg);
        VisitResult Visit(RootNode node, VisitorArgs arg);
        VisitResult Visit(StmtBlockNode node, VisitorArgs arg);
        VisitResult Visit(UnaryOperationNode node, VisitorArgs arg);
    }
}