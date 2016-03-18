using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using System;

namespace dumbo.Compiler.AST
{
    public class BreakStmtNode : StmtNode
    {
        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("Break");
        }
    }
}