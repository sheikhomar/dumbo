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
            //If .. Then
            strBuilder.Append("If (");
            Predicate.PrettyPrint(strBuilder);
            strBuilder.EndLine(") Then");

            //If Body
            Body.PrettyPrint(strBuilder);

            //Else
            ElseIfStatements.PrettyPrint(strBuilder);
            strBuilder.EndLine("Else");

            //Else body
            Else.PrettyPrint(strBuilder);

            //End If
            strBuilder.EndLine("End If");
        }
    }
}