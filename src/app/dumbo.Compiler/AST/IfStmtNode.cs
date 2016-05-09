using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class IfStmtNode : StmtNode, IHaveBlocks
    {
        public IfStmtNode(ExpressionNode predicate, StmtBlockNode then, ElseIfStmtListNode elseIfStmtList, SourcePosition sourcePosition)
        {
            Predicate = predicate;
            Then = then;
            ElseIfStatements = elseIfStmtList;
            SourcePosition = sourcePosition;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Then { get;  }
        public ElseIfStmtListNode ElseIfStatements { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
        
        public virtual IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Then;
            foreach (var elseIfStatement in ElseIfStatements)
                yield return elseIfStatement.Body;
        }
    }
}