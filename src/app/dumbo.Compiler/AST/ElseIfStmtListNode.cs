using System.Text;

namespace dumbo.Compiler.AST
{
    public class ElseIfStmtListNode : BaseListNode<ElseIfStmtNode>
    {
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}