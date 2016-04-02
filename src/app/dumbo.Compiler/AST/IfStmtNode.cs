using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class IfStmtNode : StmtNode, IHaveBlocks
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

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check Predicate
            if (Predicate.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Boolean)
                analyser.ErrorReporter.AddError("If Statement did not have a predicate of type Boolean");

            //Check body
            Body.CCAnalyse(analyser);

            //Check Else If Statements
            ElseIfStatements.CCAnalyse(analyser);
        }

        public virtual IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
            foreach (var elseIfStatement in ElseIfStatements)
                yield return elseIfStatement.Body;
        }
    }
}