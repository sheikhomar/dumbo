using System.Text;

namespace dumbo.Compiler.AST
{
    public class IdentifierNode : ExpressionNode
    {
        public IdentifierNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append(Name);
        }
    }
}