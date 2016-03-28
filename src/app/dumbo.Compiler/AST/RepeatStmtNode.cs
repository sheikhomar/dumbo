using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class RepeatStmtNode : StmtNode, IHaveBlocks
    {
        public RepeatStmtNode(ExpressionNode number, StmtBlockNode body)
        {
            Number = number;
            Body = body;
        }

        public ExpressionNode Number { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append("repeat (");
            Number.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine(")");
            Body.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine("End Repeat");
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check Predicate is of type Number
            if (Number.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Number)
                analyser.ErrorReporter.AddError("RepeatStmt must have a predicate of type Number");
            //Check Body
            Body.CCAnalyse(analyser);
        }

        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }
    }
}