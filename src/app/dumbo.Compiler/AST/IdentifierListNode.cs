using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class IdentifierListNode : BaseListNode<IdentifierNode>
    {
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}