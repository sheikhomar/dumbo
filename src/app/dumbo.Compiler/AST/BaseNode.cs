using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    abstract public class BaseNode : IPrettyPrint
    {
        virtual public void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("***" + this.ToString() + "***");
        }
    }
}