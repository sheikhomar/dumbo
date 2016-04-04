using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class IfElseStmtNode : IfStmtNode
    {
        public IfElseStmtNode(ExpressionNode predicate, StmtBlockNode body, StmtBlockNode @else, SourcePosition sourcePosition)
            : base(predicate, body, sourcePosition)
        {
            Else = @else;
        }

        public StmtBlockNode Else { get; }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
        
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            base.CCAnalyse(analyser);
            Else.CCAnalyse(analyser);
        }

        public override IEnumerable<StmtBlockNode> GetBlocks()
        {
            foreach (var block in base.GetBlocks())
                yield return block;
            yield return Else;
        }
    }
}