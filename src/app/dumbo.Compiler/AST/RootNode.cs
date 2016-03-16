using System.Collections.Generic;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class RootNode : BaseNode
    {
        public ProgramNode Program { get; set; }
        public IList<FunctionDeclNode> Functions { get; set; }

        public RootNode(ProgramNode program, IList<FunctionDeclNode> functions)
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