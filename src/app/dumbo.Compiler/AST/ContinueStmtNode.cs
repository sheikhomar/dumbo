namespace dumbo.Compiler.AST
{
    public class ContinueStmtNode : StmtNode
    {
        public ContinueStmtNode(SourcePosition sourcePosition)
        {
            base.SourcePosition = sourcePosition;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}