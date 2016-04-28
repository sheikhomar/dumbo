using System;

namespace dumbo.Compiler.AST
{
    public class ArrayValueNode : ValueNode
    {
        public ArrayValueNode(ArrayTypeNode type) : base(type)
        {
            Values = new ExpressionListNode();
        }

        public ExpressionListNode Values { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            throw new NotImplementedException();
        }
    }
}