using System.Text;

namespace dumbo.Compiler.AST
{
    public class DeclAndAssignmentStmtNode : AssignmentStmtNode
    {
        public DeclAndAssignmentStmtNode(HappyType type, 
            IdentifierListNode identifiers, 
            ExpressionListNode expressions) : base(identifiers, expressions)
        {
            Type = type;
        }

        public HappyType Type { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append(Type.ToString() + " ");
            Identifiers.PrettyPrint(StrBuilder);
            StrBuilder.Append(" := ");
            Expressions.PrettyPrint(StrBuilder);
        }
    }
}