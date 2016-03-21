using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class ReturnStmtNode : StmtNode
    {
        public ReturnStmtNode(ExpressionListNode expressions)
        {
            Expressions = expressions ?? new ExpressionListNode();
        }

        public ExpressionListNode Expressions { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append("Return ");
            if (Expressions.Count == 0)
                prettyPrinter.Append("Nothing");
            else
                Expressions.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine();
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            if (Expressions.Count == 0)
                return; //We have Nothing as return type
        }
    }
}