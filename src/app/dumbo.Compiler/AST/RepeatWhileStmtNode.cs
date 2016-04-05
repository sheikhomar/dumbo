using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class RepeatWhileStmtNode : StmtNode, IHaveBlocks
    {
        public RepeatWhileStmtNode(ExpressionNode predicate, StmtBlockNode body, SourcePosition sourcePosition)
        {
            Predicate = predicate;
            Body = body;
            SourcePosition = sourcePosition;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check predicate is bool
            if (Predicate.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Boolean)
                analyser.ErrorReporter.AddError("Repeat While Statement must have a predicate of type Boolean");

            //Check that body is type correct
            Body.CCAnalyse(analyser);
        }

        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }
    }
}