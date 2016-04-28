using System;

namespace dumbo.Compiler.AST
{
    public class ArrayDeclStmtNode : StmtNode, IVariableDeclNode
    {
        public ArrayTypeNode Type { get; }
        public IdentifierListNode Identifiers { get; }

        TypeNode IVariableDeclNode.Type => Type;

        public ArrayDeclStmtNode(ArrayTypeNode type, IdentifierListNode identifiers)
        {
            Type = type;
            Identifiers = identifiers;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}