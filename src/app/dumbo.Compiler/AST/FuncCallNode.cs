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

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.Append(Identifier + "(");
            Parameters.PrettyPrint(strBuilder);
            strBuilder.Append(")");
        }
    }
}