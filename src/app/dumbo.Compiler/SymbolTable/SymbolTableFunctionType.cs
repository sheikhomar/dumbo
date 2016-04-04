using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTableFunctionType : SymbolTableTypeEntry
    {
        public FuncDeclNode DeclarationNode { get; }
        public IList<HappyType> _parametertypes;
        public IList<HappyType> _returntypes;

        public SymbolTableFunctionType(IList<HappyType> parametertypes, IList<HappyType> returntypes)
        {
            _parametertypes = parametertypes;
            _returntypes = returntypes;
        }

        public SymbolTableFunctionType(FuncDeclNode declarationNode)
        {
            _parametertypes = declarationNode.Parameters.Select(e => e.Type).ToList();
            _returntypes = declarationNode.ReturnTypes;

            DeclarationNode = declarationNode;
        }

        public IList<HappyType> parametertypes => _parametertypes;
        public IList<HappyType> returntypes => _returntypes;
    }
}
