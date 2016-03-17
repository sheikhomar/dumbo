using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    abstract public class BaseNode : IPrettyPrint, ICCAnalysable
    {
        virtual public void CCAnalyse(ICCAnalyser analyser)
        {
            throw new NotImplementedException();
        }

        virtual public void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("***" + this.ToString() + "***");
        }

        virtual public void ScopeCheck(ISymbolTable table)
        {
            
        }
    }
}