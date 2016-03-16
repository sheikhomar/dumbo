using System.Text;

namespace dumbo.Compiler.AST
{
    public class BreakStmtNode : StmtNode
    {
        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("Break\n");
        }
    }
}