using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtNode : BaseNode
    {
        public ElseIfStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.Append("Else If (");
            Predicate.PrettyPrint(strBuilder);
            strBuilder.EndLine(") Then");
            Body.PrettyPrint(strBuilder);
        }
    }
}