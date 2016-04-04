using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTablePrimitiveType : SymbolTableTypeEntry
    {
        public IVariableDeclNode DeclarationNode { get; }

        public SymbolTablePrimitiveType(HappyType type)
        {
            Type = type;
        }

        public SymbolTablePrimitiveType(IVariableDeclNode declarationNode)
        {
            DeclarationNode = declarationNode;
            Type = declarationNode.Type;
        }

        public HappyType Type { get; set; }
    }
}
