using System.Text;

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

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("repeat while (");
            Predicate.PrettyPrint(StrBuilder);
            StrBuilder.Append(")\n");
            Body.PrettyPrint(StrBuilder);
            StrBuilder.Append("end repeat\n");
        }
    }
}