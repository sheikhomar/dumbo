using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class FunctionDeclNode : BaseNode
    {
        public IdentifierNode Identifer;
        public IList<TypeNode> ReturnTypes;
        public StmtBlockNode Body;
    }
}