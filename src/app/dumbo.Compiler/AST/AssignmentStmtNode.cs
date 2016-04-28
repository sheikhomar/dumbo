using System;

namespace dumbo.Compiler.AST
{
    public class AssignmentStmtNode : StmtNode
    {
        public AssignmentStmtNode(IdentifierListNode identifiers, ExpressionNode value, SourcePosition sourcePosition)
        {
            Identifiers = identifiers;
            Value = value;
            SourcePosition = sourcePosition;
            Expressions = new ExpressionListNode();
        }

        public IdentifierListNode Identifiers { get; }
        public ExpressionNode Value { get; }
        [Obsolete]
        public ExpressionListNode Expressions { get; set; }

        public override T Accept<T,K>(IVisitor<T,K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}