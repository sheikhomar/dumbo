namespace dumbo.Compiler.AST
{
    public class IfStmtNode : StmtNode
    {
        public ExpressionNode Predicate;
        public StmtBlockNode Body;
        public StmtBlockNode Else;
    }
}