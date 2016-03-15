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
    }
}