using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class IdentifierListNode : BaseNode
    {
        private readonly IList<IdentifierNode> _identifiers;
         
        public IdentifierListNode(IList<IdentifierNode> identifiers)
        {
            _identifiers = identifiers;
        }

        public IEnumerable<IdentifierNode> Identifiers => _identifiers;
    }
}