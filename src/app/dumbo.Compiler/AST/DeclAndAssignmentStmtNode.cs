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
    }
}