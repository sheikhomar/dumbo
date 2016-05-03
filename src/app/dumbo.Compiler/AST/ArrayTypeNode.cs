using System;

namespace dumbo.Compiler.AST
{
    public class ArrayTypeNode : TypeNode, IEquatable<ArrayTypeNode>
    {
        public PrimitiveTypeNode Type { get; set; }
        public ExpressionListNode Sizes { get; set; }

        public ArrayTypeNode(PrimitiveTypeNode type, ExpressionListNode sizes, SourcePosition srcPos)
        {
            Type = type;
            Sizes = sizes;
            SourcePosition = srcPos;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public bool Equals(ArrayTypeNode other)
        {
            if (other == null)
                return false;
            if (!Equals(Type, other.Type))
                return false;

            return true;
        }
    }
}