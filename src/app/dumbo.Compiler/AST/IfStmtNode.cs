using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class IfStmtNode : StmtNode
    {
        public IfStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
            ElseIfStatements = new ElseIfStmtListNode();
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get;  }
        public ElseIfStmtListNode ElseIfStatements { get; }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            //If ... Then
            strBuilder.Append("If (");
            Predicate.PrettyPrint(strBuilder);
            strBuilder.EndLine(") Then");
            
            //If Body
            Body.PrettyPrint(strBuilder);

            //Else If...
            ElseIfStatements.PrettyPrint(strBuilder);

            //End if
            strBuilder.EndLine("End If");
        }
    }
}