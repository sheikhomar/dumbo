using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class RootNode
    {
        public ProgramNode Program { get; set; }
        public IList<FunctionDeclNode> Functions { get; set; }

        public RootNode(ProgramNode program, IList<FunctionDeclNode> functions)
        {
            Program = program;
            Functions = functions;
        }
    }
}