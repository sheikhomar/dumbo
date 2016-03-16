using System.Text;

namespace dumbo.Compiler.AST
{
    public class ReturnStmtNode : StmtNode
    {
        public ReturnStmtNode(ExpressionListNode expressions)
        {
            Expressions = expressions ?? new ExpressionListNode();
        }

        public ExpressionListNode Expressions { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("return ");
            Expressions.PrettyPrint(StrBuilder);
            StrBuilder.Append("\n");
        }
    }
}