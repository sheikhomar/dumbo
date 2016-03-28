using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public interface IHaveBlocks
    {
        IEnumerable<StmtBlockNode> GetBlocks();
    }
}