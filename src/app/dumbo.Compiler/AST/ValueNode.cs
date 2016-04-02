using System.Text;

namespace dumbo.Compiler.AST
{
    public abstract class ValueNode : ExpressionNode
    {
        protected ValueNode(HappyType type)
        {
            Type = type;
        }

        public HappyType Type { get; }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}