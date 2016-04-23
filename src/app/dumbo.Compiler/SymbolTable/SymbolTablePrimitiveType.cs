using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTablePrimitiveType : SymbolTableTypeEntry
    {
        public IVariableDeclNode DeclarationNode { get; }

        public SymbolTablePrimitiveType(TypeNode type)
        {
            Type = type;
        }

        public SymbolTablePrimitiveType(IVariableDeclNode declarationNode)
        {
            DeclarationNode = declarationNode;
            Type = declarationNode.Type;
        }

        public TypeNode Type { get; set; }
    }
}
