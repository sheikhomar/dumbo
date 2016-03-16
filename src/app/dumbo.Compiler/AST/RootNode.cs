using System.Collections.Generic;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class RootNode : BaseNode
    {
        public ProgramNode Program { get; set; }
        public FuncDeclListNode FuncDels { get; }

        public RootNode(ProgramNode program)
        {
            Program = program;
            FuncDels = new FuncDeclListNode();
        }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            Program.PrettyPrint(StrBuilder);
            if (FuncDels != null)
            {
                foreach (var node in FuncDels)
                {
                    node.PrettyPrint(StrBuilder);
                }
            }
        }
    }
}