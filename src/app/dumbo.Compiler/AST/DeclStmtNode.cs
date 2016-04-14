﻿namespace dumbo.Compiler.AST
{
    public class DeclStmtNode : StmtNode, IVariableDeclNode
    {
        public DeclStmtNode(IdentifierListNode identifiers, HappyType type, SourcePosition sourcePosition)
        {
            Identifiers = identifiers;
            Type = type;
            SourcePosition = sourcePosition;
        }

        public IdentifierListNode Identifiers { get; }
        public HappyType Type { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}