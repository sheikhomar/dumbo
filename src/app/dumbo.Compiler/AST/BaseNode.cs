using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public abstract class BaseNode : IPrettyPrint, ICCAnalysable, IVisitable
    {
        public abstract void CCAnalyse(ICCAnalyser analyser);

        public virtual void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("***" + this.ToString() + "***");
        }

        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public abstract VisitResult Accept(IVisitor visitor, VisitorArgs arg);
    }
}