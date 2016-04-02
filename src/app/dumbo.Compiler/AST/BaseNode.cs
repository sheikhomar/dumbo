using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public abstract class BaseNode : ICCAnalysable, IVisitable
    {
        public abstract void CCAnalyse(ICCAnalyser analyser);
        
        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public abstract VisitResult Accept(IVisitor visitor, VisitorArgs arg);
    }
}