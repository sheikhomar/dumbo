using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public abstract class ExpressionNode : BaseNode
    {
        public abstract TypeDescriptor GetHappyType(ISymbolTable symbolTable);
    }
}