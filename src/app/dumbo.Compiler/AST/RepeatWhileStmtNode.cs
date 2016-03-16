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

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.Append("repeat while (");
            Predicate.PrettyPrint(strBuilder);
            strBuilder.EndLine(")");
            Body.PrettyPrint(strBuilder);
            strBuilder.EndLine("end repeat");
        }
    }
}