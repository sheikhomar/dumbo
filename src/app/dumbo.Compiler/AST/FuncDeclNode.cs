using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class FuncDeclNode : BaseNode
    {
        public FuncDeclNode(IdentifierNode identifer, StmtBlockNode body)
        {
            Identifer = identifer;
            Body = body;
            ReturnTypes = new List<HappyType>();
            Parameters = new FormalParamListNode();
        }

        public FormalParamListNode Parameters { get; }
        public IdentifierNode Identifer { get; }
        public IList<HappyType> ReturnTypes { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.EndLine();
            strBuilder.EndLine();
            strBuilder.Append("Function " + Identifer.Name + "(");
            Parameters.PrettyPrint(strBuilder);
            strBuilder.Append(") returns ");

            if (ReturnTypes.Count == 0)
                strBuilder.Append("Nothing");
            else if (ReturnTypes.Count != 1)
            {
                strBuilder.Append(ReturnTypes[0].ToString());

                for (int i = 1; i < ReturnTypes.Count; i++)
                {
                    strBuilder.Append(ReturnTypes[i].ToString());
                }
            }
            else
                strBuilder.Append(ReturnTypes[0].ToString());
        }
    }
}