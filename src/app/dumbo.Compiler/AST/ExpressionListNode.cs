using System.Text;

namespace dumbo.Compiler.AST
{
    public class ExpressionListNode : BaseListNode<ExpressionNode>
    {
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}