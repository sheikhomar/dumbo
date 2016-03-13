using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class RootNode
    {
        public ProgramNode Program { get; set; }
        public IList<FunctionDeclarationNode> Functions { get; set; }

        public RootNode(ProgramNode program, IList<FunctionDeclarationNode> functions)
        {
            Program = program;
            Functions = functions;
        }
    }
}