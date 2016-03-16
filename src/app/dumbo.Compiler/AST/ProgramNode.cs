using System.Linq.Expressions;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : BaseNode
    {
        public ProgramNode(StmtBlockNode body)
        {
            Body = body;
        }

        public StmtBlockNode Body { get; }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("Program \r\n");
            Body.PrettyPrint(StrBuilder);
            StrBuilder.Append("End Program \r\n");
        }
    }

}