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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            //Declares the Function + its name
            prettyPrinter.EndLine();
            prettyPrinter.EndLine();
            prettyPrinter.Append("Function " + Identifer.Name + "(");
            Parameters.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(") Returns ");

            //Declare the return types of the function decl
            if (ReturnTypes.Count == 0)
                prettyPrinter.Append("Nothing");
            else if (ReturnTypes.Count != 1)
            {
                prettyPrinter.Append(ReturnTypes[0].ToString());

                for (int i = 1; i < ReturnTypes.Count; i++)
                {
                    prettyPrinter.Append(", " + ReturnTypes[i].ToString());
                }
            }
            else
                prettyPrinter.Append(ReturnTypes[0].ToString());

            prettyPrinter.EndLine();

            Body.PrettyPrint(prettyPrinter);

            //Finish the function with end
            prettyPrinter.EndLine("End Function");

        }
    }
}