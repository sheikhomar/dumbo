using System.Text;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtListNode : BaseListNode<ElseIfStmtNode>
    {
        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            if (this == null)
                return;

            foreach (var node in this)
            {
                node.PrettyPrint(StrBuilder);
                StrBuilder.Append("\n");
            }
        }
    }
}