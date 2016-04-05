using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class IdentifierListNode : BaseListNode<IdentifierNode>
    {
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}