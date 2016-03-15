using System.Linq.Expressions;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : BaseNode
    {
        public ProgramNode(StmtBlockNode body)
        {
            Body = body;
        }

        public  StmtBlockNode Body { get; }
    }

}