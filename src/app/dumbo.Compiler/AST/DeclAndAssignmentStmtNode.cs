namespace dumbo.Compiler.AST
{
    public class DeclAndAssignmentStmtNode : AssignmentStmtNode, IVariableDeclNode
    {
        public DeclAndAssignmentStmtNode(HappyType type, 
            IdentifierListNode identifiers, 
            ExpressionListNode expressions,
            SourcePosition sourcePosition) : base(identifiers, expressions, sourcePosition)
        {
            Type = type;
        }

        public HappyType Type { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}