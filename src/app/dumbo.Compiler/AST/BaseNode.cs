using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public abstract class BaseNode : ICCAnalysable, IVisitable
    {
        protected BaseNode()
        {
            SourcePosition = new SourcePosition(0, 0, 0, 0);
        }

        public abstract void CCAnalyse(ICCAnalyser analyser);
        
        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public SourcePosition SourcePosition { get; set; }
        public abstract T Accept<T,K>(IVisitor<T,K> visitor, K arg);
    }
}