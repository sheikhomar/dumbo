namespace dumbo.Compiler.AST
{
    public class RepeatWhileStmtNode : StmtNode
    {
        public ExpressionNode Predicate;
        public StmtBlockNode Body;
    }
}