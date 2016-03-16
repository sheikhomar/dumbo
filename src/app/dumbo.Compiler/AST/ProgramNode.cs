using System.Linq.Expressions;
using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : BaseNode
    {
        public ProgramNode(StmtBlockNode body)
        {
            Body = body;
        }

        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            strBuilder.EndLine("Program");
            Body.PrettyPrint(strBuilder);
            strBuilder.EndLine("End Program");
        }
    }

}