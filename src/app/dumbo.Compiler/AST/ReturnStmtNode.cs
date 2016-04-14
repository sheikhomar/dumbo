namespace dumbo.Compiler.AST
{
    public class ReturnStmtNode : StmtNode
    {
        public ReturnStmtNode(ExpressionListNode expressions, SourcePosition srcPos)
        {
            Expressions = expressions ?? new ExpressionListNode();
            SourcePosition = srcPos;
        }

        public ExpressionListNode Expressions { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}