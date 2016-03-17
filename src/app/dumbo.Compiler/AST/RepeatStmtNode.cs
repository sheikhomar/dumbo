using System.Text;
using dumbo.Compiler.PrettyPrint;

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

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.Append("repeat (");
            Number.PrettyPrint(strBuilder);
            strBuilder.EndLine(")");
            Body.PrettyPrint(strBuilder);
            strBuilder.EndLine("End Repeat");
        }
    }
}