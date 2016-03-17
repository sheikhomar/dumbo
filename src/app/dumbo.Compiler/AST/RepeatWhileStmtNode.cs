using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class RepeatWhileStmtNode : StmtNode
    {
        public RepeatWhileStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append("repeat while (");
            Predicate.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine(")");
            Body.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine("end repeat");
        }
    }
}