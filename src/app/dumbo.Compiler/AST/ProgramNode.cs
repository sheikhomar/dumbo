using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : BaseNode, IHaveBlocks
    {
        public ProgramNode(StmtBlockNode body, SourcePosition srcPos)
        {
            Body = body;
            SourcePosition = srcPos;
        }

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