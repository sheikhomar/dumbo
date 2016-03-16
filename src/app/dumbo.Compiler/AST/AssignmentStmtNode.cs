using System.Collections.Generic;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class AssignmentStmtNode : StmtNode
    {
        public AssignmentStmtNode(IdentifierListNode identifiers, ExpressionListNode expressions)
        {
            Identifiers = identifiers;
            Expressions = expressions;
        }

        public IdentifierListNode Identifiers { get; }
        public ExpressionListNode Expressions { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            Identifiers.PrettyPrint(StrBuilder);

            StrBuilder.Append(" := ");

            Expressions.PrettyPrint(StrBuilder);

            StrBuilder.Append("\n");
        }
    }
}