namespace dumbo.Compiler.AST
{
    public class FormalParamListNode : BaseListNode<FormalParamNode>
    {
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}