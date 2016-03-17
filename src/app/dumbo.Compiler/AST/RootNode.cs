using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

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

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            Program.PrettyPrint(prettyPrinter);
            FuncDecls.PrettyPrint(prettyPrinter);
        }

        public override void ScopeCheck(ISymbolTable table)
        {
            Program.ScopeCheck(table);
            FuncDecls.ScopeCheck(table);
        }
    }
}