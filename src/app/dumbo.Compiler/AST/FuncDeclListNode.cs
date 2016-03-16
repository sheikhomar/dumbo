using System.Text;

namespace dumbo.Compiler.AST
{
    public class FuncDeclListNode : BaseListNode<FuncDeclNode>
    {
        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            if (this != null)
            {
                foreach (var node in this)
                {
                    node.PrettyPrint(StrBuilder);
                }
            }
        }
    }
}