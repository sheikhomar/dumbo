namespace dumbo.Compiler.AST
{
    public class ElseIfStmtNode : BaseNode
    {
        public ElseIfStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get; }
    }
}