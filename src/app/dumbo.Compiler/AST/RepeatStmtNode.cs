using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class RepeatStmtNode : StmtNode, IHaveBlocks
    {
        public RepeatStmtNode(ExpressionNode number, StmtBlockNode body, SourcePosition sourcePosition)
        {
            Number = number;
            Body = body;
            SourcePosition = sourcePosition;
        }

        public ExpressionNode Number { get; }
        public StmtBlockNode Body { get; }
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check Predicate is of type Number
            if (Number.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Number)
                analyser.ErrorReporter.AddError("RepeatStmt must have a predicate of type Number");
            //Check Body
            Body.CCAnalyse(analyser);
        }

        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }
    }
}