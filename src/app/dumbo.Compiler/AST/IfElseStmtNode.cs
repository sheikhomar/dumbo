using System.Text;
using dumbo.Compiler.CCAnalysis;
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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            //If .. Then
            prettyPrinter.Append("If (");
            Predicate.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine(") Then");

            //If Body
            Body.PrettyPrint(prettyPrinter);

            //Else
            ElseIfStatements.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine("Else");

            //Else body
            Else.PrettyPrint(prettyPrinter);

            //End If
            prettyPrinter.EndLine("End If");
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check predicate
            if (Predicate.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Boolean)
                analyser.ErrorReporter.AddError("IfElse Statement did not have a predicate of type Boolean");

            //Check body
            Body.CCAnalyse(analyser);
            Else.CCAnalyse(analyser);
        }
    }
}