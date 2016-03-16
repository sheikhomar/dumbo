using System.Text;

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

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append(Type.ToString() + " ");
            Identifiers.PrettyPrint(StrBuilder);
        }
    }
}