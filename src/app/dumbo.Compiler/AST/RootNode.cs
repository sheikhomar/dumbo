using System.Collections.Generic;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class RootNode : BaseNode
    {
        public ProgramNode Program { get; set; }
        public IList<FuncDelcNode> Functions { get; set; }

        public RootNode(ProgramNode program, IList<FuncDelcNode> functions)
        {
            Program = program;
            Functions = functions;
        }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            Program.PrettyPrint(StrBuilder);
            if (Functions != null)
            {
                foreach (var node in Functions)
                {
                    node.PrettyPrint(StrBuilder);
                }
            }
        }
    }
}