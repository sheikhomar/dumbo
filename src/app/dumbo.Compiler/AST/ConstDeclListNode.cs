using System;

namespace dumbo.Compiler.AST
{
    public class ConstDeclListNode : BaseListNode<ConstDeclNode>
    {
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}