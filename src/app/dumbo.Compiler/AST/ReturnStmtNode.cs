namespace dumbo.Compiler.AST
{
    public class ReturnStmtNode : StmtNode
    {
        public ReturnStmtNode(ExpressionListNode expressions)
        {
            Expressions = expressions ?? new ExpressionListNode();
        }

        public ExpressionListNode Expressions { get; }
    }
}