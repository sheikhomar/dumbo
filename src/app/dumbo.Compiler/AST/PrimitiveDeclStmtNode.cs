namespace dumbo.Compiler.AST
{
    public class PrimitiveDeclStmtNode : StmtNode, IVariableDeclNode
    {
        public PrimitiveDeclStmtNode(IdentifierListNode identifiers, TypeNode type, SourcePosition sourcePosition)
        {
            Identifiers = identifiers;
            Type = type;
            SourcePosition = sourcePosition;
        }

        public IdentifierListNode Identifiers { get; }

        public TypeNode Type { get; }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}