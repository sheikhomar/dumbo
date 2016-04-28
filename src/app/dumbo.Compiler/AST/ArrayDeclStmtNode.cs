namespace dumbo.Compiler.AST
{
    public class ArrayDeclStmtNode : StmtNode
    {
        public ArrayTypeNode Type { get; }
        public IdentifierListNode Identifiers { get; }


        public ArrayDeclStmtNode(ArrayTypeNode type, IdentifierListNode identifiers)
        {
            Type = type;
            Identifiers = identifiers;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            throw new System.NotImplementedException();
        }
    }
}