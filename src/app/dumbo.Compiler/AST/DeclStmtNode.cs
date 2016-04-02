using System.Text;
using dumbo.Compiler.CCAnalysis;
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
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            foreach (var id in Identifiers)
            {
                analyser.AddVariableToSymbolTable(id.Name, Type, false, Line, Column);
            }

            //
        }


    }
}