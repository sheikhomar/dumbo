namespace dumbo.Compiler.AST
{
    public class RepeatStmtNode : StmtNode
    {
        public ExpressionNode Number;
        public StmtBlockNode Body;
    }
}