using System.Text;
using dumbo.Compiler.CCAnalysis;
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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check predicate is bool
            if (Predicate.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Boolean)
                analyser.ErrorReporter.AddError("Predicate is not of type Boolean");

            //Check that body is type correct
            Body.CCAnalyse(analyser);
        }
    }
}