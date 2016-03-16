using System.Text;

namespace dumbo.Compiler.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public FuncCallStmtNode(FuncCallNode funcCallNode)
        {
            CallNode = funcCallNode;
        }

        public FuncCallNode CallNode { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            CallNode.PrettyPrint(StrBuilder);
        }
    }
}