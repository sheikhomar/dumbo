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
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}