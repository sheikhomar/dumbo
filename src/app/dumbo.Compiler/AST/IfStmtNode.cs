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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            //If ... Then
            prettyPrinter.Append("If (");
            Predicate.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine(") Then");
            
            //If Body
            Body.PrettyPrint(prettyPrinter);

            //Else If...
            ElseIfStatements.PrettyPrint(prettyPrinter);

            //End if
            prettyPrinter.EndLine("End If");
        }
    }
}