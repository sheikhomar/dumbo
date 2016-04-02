using System.Text;

namespace dumbo.Compiler.AST
{
    public class FuncDeclListNode : BaseListNode<FuncDeclNode>
    {
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}