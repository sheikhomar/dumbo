using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtListNode : BaseListNode<ElseIfStmtNode>
    {
        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            if (this == null)
                return;

            foreach (var node in this)
            {
                node.PrettyPrint(prettyPrinter);
            }
        }
    }
}