using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class BreakStmtNode : StmtNode
    {
        public override void PrettyPrint(IPrettyPrinter StrBuilder)
        {
            StrBuilder.EndLine("Break");
        }
    }
}