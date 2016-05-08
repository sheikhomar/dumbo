using System;

namespace dumbo.Compiler.AST
{
    public class PrimitiveTypeNode : TypeNode, IEquatable<PrimitiveTypeNode>
    {
        public PrimitiveType Type { get; }

        public PrimitiveTypeNode(PrimitiveType type, SourcePosition srcPos = null)
        {
            Type = type;
            SourcePosition = srcPos;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PrimitiveTypeNode);
        }

        public bool Equals(PrimitiveTypeNode other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Type == other.Type;
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