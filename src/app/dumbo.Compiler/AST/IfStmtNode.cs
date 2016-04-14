using System.Collections.Generic;

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

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
        
        public virtual IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
            foreach (var elseIfStatement in ElseIfStatements)
                yield return elseIfStatement.Body;
        }
    }
}