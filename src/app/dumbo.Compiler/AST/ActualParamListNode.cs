namespace dumbo.Compiler.AST
{
    public class ActualParamListNode : BaseListNode<ExpressionNode>
    {
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}