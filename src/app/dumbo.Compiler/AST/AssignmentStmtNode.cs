namespace dumbo.Compiler.AST
{
    public class AssignmentStmtNode : StmtNode
    {
        public AssignmentStmtNode(IdentifierListNode identifiers, ExpressionListNode expressions, SourcePosition sourcePosition)
        {
            Identifiers = identifiers;
            Expressions = expressions;
            SourcePosition = sourcePosition;
        }

        public IdentifierListNode Identifiers { get; }
        public ExpressionListNode Expressions { get; }
        
        public override T Accept<T,K>(IVisitor<T,K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}