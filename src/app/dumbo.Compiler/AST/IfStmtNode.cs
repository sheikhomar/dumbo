using System.Text;

namespace dumbo.Compiler.AST
{
    public class IfStmtNode : StmtNode
    {
        public IfStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
            ElseIfStatements = new ElseIfStmtListNode();
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get;  }
        public ElseIfStmtListNode ElseIfStatements { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("if (");
            Predicate.PrettyPrint(StrBuilder);
            StrBuilder.Append(")\n");
            Body.PrettyPrint(StrBuilder);
            ElseIfStatements.PrettyPrint(StrBuilder);
            StrBuilder.Append("end if\n");
        }
    }
}