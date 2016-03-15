using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseNode
    {
        public StmtBlockNode()
        {
            Body = new List<StmtNode>();
        }

        public IList<StmtNode> Body { get; }

        
    }
}