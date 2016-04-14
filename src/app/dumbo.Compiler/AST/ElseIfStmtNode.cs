using System.Collections.Generic;

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