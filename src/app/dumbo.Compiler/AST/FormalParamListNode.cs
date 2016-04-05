namespace dumbo.Compiler.AST
{
    public class FormalParamListNode : BaseListNode<FormalParamNode>
    {
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}