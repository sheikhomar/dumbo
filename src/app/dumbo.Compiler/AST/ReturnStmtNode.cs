using System.Text;
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

        public override void PrettyPrint(IPrettyPrinter StrBuilder)
        {
            StrBuilder.Append("return ");
            Expressions.PrettyPrint(StrBuilder);
            StrBuilder.EndLine();
        }
    }
}