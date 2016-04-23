using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTableFunctionType : SymbolTableTypeEntry
    {
        public FuncDeclNode DeclarationNode { get; }

        public SymbolTableFunctionType(FuncDeclNode declarationNode)
        {
            DeclarationNode = declarationNode;
        }
    }
}
