namespace dumbo.Compiler.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public FuncCallStmtNode(FuncCallNode funcCallNode)
        {
            CallNode = funcCallNode;
        }

        public FuncCallNode CallNode { get; }
    }
}