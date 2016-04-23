using System;

namespace dumbo.Compiler.AST
{
    public class PrimitiveTypeNode : TypeNode, IEquatable<PrimitiveTypeNode>
    {
        public PrimitiveType Type { get; }

        public PrimitiveTypeNode(PrimitiveType type)
        {
            Type = type;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as PrimitiveTypeNode);
        }

        public bool Equals(PrimitiveTypeNode other)
        {
            return Type == other?.Type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}