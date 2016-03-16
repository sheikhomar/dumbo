using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class FuncDeclListNode : BaseListNode<FuncDeclNode>
    {
        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            if (this != null)
            {
                foreach (var node in this)
                {
                    node.PrettyPrint(strBuilder);
                }
            }
        }
    }
}