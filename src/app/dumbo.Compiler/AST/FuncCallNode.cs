using System.Text;

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

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append(Identifier + "(");
            Parameters.PrettyPrint(StrBuilder);
            StrBuilder.Append(")\n");
        }
    }
}