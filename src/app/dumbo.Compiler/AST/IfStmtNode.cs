using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class IfStmtNode : StmtNode, IHaveBlocks
    {
        public IfStmtNode(ExpressionNode predicate, StmtBlockNode body, SourcePosition sourcePosition)
        {
            Predicate = predicate;
            Body = body;
            ElseIfStatements = new ElseIfStmtListNode();
            SourcePosition = sourcePosition;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get;  }
        public ElseIfStmtListNode ElseIfStatements { get; }
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check Predicate
            if (Predicate.GetHappyType(analyser.SymbolTable).GetFirst() != HappyType.Boolean)
                analyser.ErrorReporter.AddError("If Statement did not have a predicate of type Boolean");

            //Check body
            Body.CCAnalyse(analyser);

            //Check Else If Statements
            ElseIfStatements.CCAnalyse(analyser);
        }

        public virtual IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
            foreach (var elseIfStatement in ElseIfStatements)
                yield return elseIfStatement.Body;
        }
    }
}