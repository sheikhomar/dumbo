using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class DeclStmtNode : StmtNode
    {
        public DeclStmtNode(IdentifierListNode identifiers, HappyType type)
        {
            Identifiers = identifiers;
            Type = type;
        }

        public IdentifierListNode Identifiers { get; }
        public HappyType Type { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Type.ToString() + " ");
            Identifiers.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine();
        }

        public override void ScopeCheck(ISymbolTable table)
        {
            base.ScopeCheck(table);
        }
    }
}