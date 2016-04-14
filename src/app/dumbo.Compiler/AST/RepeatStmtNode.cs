using System.Collections.Generic;

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

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }
    }
}