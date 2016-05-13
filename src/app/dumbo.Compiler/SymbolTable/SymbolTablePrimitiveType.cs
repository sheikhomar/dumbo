using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTablePrimitiveType : SymbolTableTypeEntry
    {
        public IVariableDeclNode DeclarationNode { get; }
        public bool IsReadonly { get; }

        public SymbolTablePrimitiveType(TypeNode type)
        {
            Type = type;
        }

        public SymbolTablePrimitiveType(IVariableDeclNode declarationNode, bool isReadonly = false)
        {
            DeclarationNode = declarationNode;
            IsReadonly = isReadonly;
            Type = declarationNode.Type;
        }

        public TypeNode Type { get; set; }
    }
}
