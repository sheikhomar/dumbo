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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append("repeat (");
            Number.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine(")");
            Body.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine("End Repeat");
        }
    }
}