using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public FuncCallStmtNode(FuncCallNode funcCallNode)
        {
            CallNode = funcCallNode;
        }

        public FuncCallNode CallNode { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            CallNode.PrettyPrint(prettyPrinter);
        }
    }
}