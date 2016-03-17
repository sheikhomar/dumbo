using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class IdentifierNode : ExpressionNode
    {
        public IdentifierNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Name);
        }
    }
}