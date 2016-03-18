using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

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

        public override void CCAnalyse(ICCAnalyser analyser)
        {

            //Check all Type and literal, remember that a, b = 5 ie last exp
            //This must be done before scope stuff.

            var idList = Identifiers.GetAllAs<IdentifierNode>();

            foreach (var id in idList)
            {
                analyser.SymbolTable.EnterSymbol(id.Name, new SymbolTablePrimitiveType(Type));
            }
        }
    }
}