using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public FuncCallStmtNode(FuncCallExprNode funcCallNode)
        {
            CallNode = funcCallNode;
        }

        public FuncCallExprNode CallNode { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            CallNode.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine();
        }
    }
}