using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class RootNode : BaseNode
    {
        public ProgramNode Program { get; set; }
        public FuncDeclListNode FuncDecls { get; }

        public RootNode(ProgramNode program)
        {
            Program = program;
            FuncDecls = new FuncDeclListNode();
        }

        public override void PrettyPrint(IPrettyPrinter strBuilder)
        {
            Program.PrettyPrint(strBuilder);
            FuncDecls.PrettyPrint(strBuilder);
        }
    }
}