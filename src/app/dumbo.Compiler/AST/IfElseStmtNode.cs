using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class IfElseStmtNode : IfStmtNode
    {
        public IfElseStmtNode(ExpressionNode predicate, StmtBlockNode body, StmtBlockNode @else)
            : base(predicate, body)
        {
            Else = @else;
        }

        public StmtBlockNode Else { get; }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.Append("If (");
            Predicate.PrettyPrint(strBuilder);
            strBuilder.EndLine(") Then");
            Body.PrettyPrint(strBuilder);
            ElseIfStatements.PrettyPrint(strBuilder);
            strBuilder.EndLine("Else");
            Else.PrettyPrint(strBuilder);
            strBuilder.EndLine("End If");
        }
    }
}