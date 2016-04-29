using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public interface IVisitor<T, K>
    {
        T Visit(ActualParamListNode node, K arg);
        T Visit(AssignmentStmtNode node, K arg);
        T Visit(BinaryOperationNode node, K arg);
        T Visit(BreakStmtNode node, K arg);
        T Visit(BuiltInFuncDeclNode node, K arg);
        T Visit(DeclAndAssignmentStmtNode node, K arg);
        T Visit(DeclStmtNode node, K arg);
        T Visit(ElseIfStmtListNode node, K arg);
        T Visit(ElseIfStmtNode node, K arg);
        T Visit(ExpressionListNode node, K arg);
        T Visit(FormalParamListNode node, K arg);
        T Visit(FormalParamNode node, K arg);
        T Visit(FuncCallExprNode node, K arg);
        T Visit(FuncCallStmtNode node, K arg);
        T Visit(FuncDeclListNode node, K arg);
        T Visit(FuncDeclNode node, K arg);
        T Visit(IdentifierListNode node, K arg);
        T Visit(IdentifierNode node, K arg);
        T Visit(IfElseStmtNode node, K arg);
        T Visit(IfStmtNode node, K arg);
        T Visit(LiteralValueNode node, K arg);
        T Visit(PrimitiveTypeNode node, K arg);
        T Visit(ProgramNode node, K arg);
        T Visit(RepeatStmtNode node, K arg);
        T Visit(RepeatWhileStmtNode node, K arg);
        T Visit(ReturnStmtNode node, K arg);
        T Visit(RootNode node, K arg);
        T Visit(StmtBlockNode node, K arg);
        T Visit(UnaryOperationNode node, K arg);
    }
}
