using System.Text;

namespace dumbo.Compiler.AST
{
    public class RepeatStmtNode : StmtNode
    {
        public RepeatStmtNode(ExpressionNode number, StmtBlockNode body)
        {
            Number = number;
            Body = body;
        }

        public ExpressionNode Number { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("repeat (" + Number + ")\n");
            Body.PrettyPrint(StrBuilder);
            StrBuilder.Append("end repeat\n");
        }
    }
}