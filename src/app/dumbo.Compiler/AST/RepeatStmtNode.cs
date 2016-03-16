using System.Text;

namespace dumbo.Compiler.AST
{
    public class RepeatStmtNode : StmtNode
    {
        public ExpressionNode Number;
        public StmtBlockNode Body;

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("repeat (" + Number + ")\n");
            Body.PrettyPrint(StrBuilder);
            StrBuilder.Append("end repeat\n");
        }
    }
}