using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtNode : BaseNode, IHaveBlocks
    {
        public ElseIfStmtNode(ExpressionNode predicate, StmtBlockNode body, SourcePosition sourcePosition)
        {
            Predicate = predicate;
            Body = body;
            SourcePosition = sourcePosition;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get; }
        
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check Predicate
            if (Predicate.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Boolean)
                analyser.ErrorReporter.AddError("Else If Statement did not have a predicate of type Boolean");

            //Check body
            Body.CCAnalyse(analyser);
        }

        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}