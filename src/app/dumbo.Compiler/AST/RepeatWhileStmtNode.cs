using System.Text;

namespace dumbo.Compiler.AST
{
    public class RepeatWhileStmtNode : StmtNode
    {
        public ExpressionNode Predicate;
        public StmtBlockNode Body;

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