using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class FuncDeclNode : BaseNode
    {
        public FuncDeclNode(IdentifierNode identifer, StmtBlockNode body)
        {
            Identifer = identifer;
            Body = body;
            ReturnTypes = new List<HappyType>();
        }

        public IdentifierNode Identifer { get; }
        public IList<HappyType> ReturnTypes { get; }
        public StmtBlockNode Body { get; }
    }
}