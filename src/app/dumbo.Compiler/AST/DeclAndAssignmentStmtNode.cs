﻿namespace dumbo.Compiler.AST
{
    public class DeclAndAssignmentStmtNode : AssignmentStmtNode, IVariableDeclNode
    {
        public DeclAndAssignmentStmtNode(TypeNode type, 
            IdentifierListNode identifiers, 
            ExpressionNode value,
            SourcePosition sourcePosition) : base(identifiers, value, sourcePosition)
        {
            Type = type;
        }

        public TypeNode Type { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}