namespace dumbo.Compiler.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public FuncCallStmtNode(FuncCallExprNode funcCallNode)
        {
            CallNode = funcCallNode;
        }

        public FuncCallExprNode CallNode { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}