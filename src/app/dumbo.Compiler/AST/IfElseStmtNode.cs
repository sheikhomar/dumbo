using System.Text;

namespace dumbo.Compiler.AST
{
    public class IfElseStmtNode : IfStmtNode
    {
        public IfElseStmtNode(ExpressionNode predicate, StmtBlockNode body, StmtBlockNode @else)
            : base(predicate, body)
        {
            Else = @else;
        }

        public StmtBlockNode Else { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("if (");
            Predicate.PrettyPrint(StrBuilder);
            StrBuilder.Append(")\n");
            Body.PrettyPrint(StrBuilder);
            ElseIfStatements.PrettyPrint(StrBuilder);
            StrBuilder.Append("Else\n");
            Else.PrettyPrint(StrBuilder);
            StrBuilder.Append("end if\n");
        }
    }
}