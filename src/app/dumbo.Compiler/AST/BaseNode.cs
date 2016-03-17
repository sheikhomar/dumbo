using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;
using dumbo.Compiler.ScopeCheck;

namespace dumbo.Compiler.AST
{
    abstract public class BaseNode : IPrettyPrint , IScopeCheck
    {
        virtual public void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("***" + this.ToString() + "***");
        }

        virtual public void ScopeCheck(ISymbolTable table)
        {
            
        }
    }
}