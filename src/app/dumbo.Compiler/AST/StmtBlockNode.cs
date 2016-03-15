using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseNode
    {
        public IList<StmtNode> Body;
    }
}