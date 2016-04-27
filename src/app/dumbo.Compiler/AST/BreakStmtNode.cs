using System.Globalization;

namespace dumbo.Compiler.AST
{
    public class BreakStmtNode : StmtNode
    {
        public BreakStmtNode(SourcePosition sourcePosition)
        {
            base.SourcePosition = sourcePosition;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}