namespace dumbo.Compiler.AST
{
    public class ElseIfStmtNode : BaseNode
    {
        public ExpressionNode Predicate { get; set; }
        public StmtBlockNode Body { get; set; }
    }
}