using System.Text;
using dumbo.Compiler.CCAnalysis;
using System;

namespace dumbo.Compiler.AST
{
    public class BreakStmtNode : StmtNode
    {
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            // Do nothing
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}