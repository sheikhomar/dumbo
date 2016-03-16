using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseListNode<StmtNode>
    {
        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            foreach (var node in this.GetAllAs<StmtNode>())
            {
                node.PrettyPrint(strBuilder);
            }
        }
    }
}