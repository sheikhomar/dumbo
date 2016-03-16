using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseListNode<StmtNode>
    {
        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            var newList = GetAllAs<StmtNode>();

            if (newList.Count == 0)
                return;

            foreach (var node in newList)
            {
                node.PrettyPrint(strBuilder);
            }
        }
    }
}