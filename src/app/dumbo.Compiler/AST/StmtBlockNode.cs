using System.Text;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseListNode<StmtNode>
    {
        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            foreach (var node in this.GetAllAs<StmtNode>())
            {
                node.PrettyPrint(StrBuilder);
            }
        }
    }
}