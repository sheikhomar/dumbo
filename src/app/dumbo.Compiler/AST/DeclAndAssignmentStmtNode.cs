using System.Text;
using dumbo.Compiler.PrettyPrint;

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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Type.ToString() + " ");
            Identifiers.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(" := ");
            Expressions.PrettyPrint(prettyPrinter);
        }
    }
}