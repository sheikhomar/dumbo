using dumbo.Compiler.PrettyPrint;
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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            Identifiers.PrettyPrint(prettyPrinter);

            prettyPrinter.Append(" := ");

            Expressions.PrettyPrint(prettyPrinter);

            prettyPrinter.EndLine();
        }
    }
}