using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public abstract class BaseNode : IPrettyPrint, ICCAnalysable
    {
        public virtual void CCAnalyse(ICCAnalyser analyser)
        {
            throw new NotImplementedException(); //should make CCAnalyse abstract
        }

        public virtual void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("***" + this.ToString() + "***");
        }
    }
}