using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class FuncCallNode : ExpressionNode
    {
        public FuncCallNode(string identifier)
        {
            Identifier = identifier;
            Parameters = new ActualParamListNode();
        }

        public string Identifier { get; }
        public ActualParamListNode Parameters { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Identifier + "(");
            Parameters.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(")");
        }
    }
}