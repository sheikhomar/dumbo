using dumbo.Compiler.PrettyPrint;
using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.SymbolTable;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class AssignmentStmtNode : StmtNode
    {
        public AssignmentStmtNode(IdentifierListNode identifiers, ExpressionListNode expressions)
        {
            Identifiers = identifiers;
            Expressions = expressions;
        }

        public IdentifierListNode Identifiers { get; }
        public ExpressionListNode Expressions { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            Identifiers.PrettyPrint(prettyPrinter);

            prettyPrinter.Append(" := ");

            Expressions.PrettyPrint(prettyPrinter);

            prettyPrinter.EndLine();
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            // TODO: Implement this logic, it has to check for when many identifiers and 1 expresision
            if (Identifiers.Count != Expressions.Count)
                analyser.ErrorReporter.AddError("Too many identifiers than expressions in stmt, or reverse");

            IList<HappyType> exprTypes = new List<HappyType>();

            foreach (var expr in Expressions)
            {
                expr.CCAnalyse(analyser);
            }

            analyser.IsListsEqual(Identifiers.GetAllAs<IdentifierNode>, Expressions.);
        }
    }
}