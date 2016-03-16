using System.Collections.Generic;
using System.Text;

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

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("\n\n" + "Function " + Identifer.Name + "(");
            Parameters.PrettyPrint(StrBuilder);
            StrBuilder.Append(") returns ");

            if (ReturnTypes.Count == 0)
                StrBuilder.Append("Nothing");
            else if (ReturnTypes.Count != 1)
            {
                StrBuilder.Append(ReturnTypes[0].ToString());

                for (int i = 1; i < ReturnTypes.Count; i++)
                {
                    StrBuilder.Append(ReturnTypes[i].ToString());
                }
            }
            else
                StrBuilder.Append(ReturnTypes[0].ToString());
        }
    }
}