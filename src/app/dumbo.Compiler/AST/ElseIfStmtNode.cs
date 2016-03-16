using System.Text;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtNode : BaseNode
    {
        public ElseIfStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("else if (");
            Predicate.PrettyPrint(StrBuilder);
            StrBuilder.Append(")\n");
            Body.PrettyPrint(StrBuilder);
        }
    }
}