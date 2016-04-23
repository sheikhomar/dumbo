using System.Text;

namespace dumbo.Compiler.AST
{
    public abstract class ValueNode : ExpressionNode
    {
        protected ValueNode(TypeNode type)
        {
            Type = type;
        }

        public TypeNode Type { get; }
    }
}