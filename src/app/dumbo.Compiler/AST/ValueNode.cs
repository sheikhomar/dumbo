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
    }
}