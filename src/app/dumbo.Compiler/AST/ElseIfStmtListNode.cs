using System.Text;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtListNode : BaseListNode<ElseIfStmtNode>
    {
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}