using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class IdentifierNode : ExpressionNode
    {
        public IdentifierNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Name);
        }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            var typeDescriptor = new TypeDescriptor();
            
            HappyType? retrivedType = null;
            var symbolEntry = symbolTable.RetrieveSymbol(Name);
            if (symbolEntry != null)
            {
                retrivedType = (symbolEntry.Type as SymbolTablePrimitiveType)?.Type;
            }

            if (retrivedType.HasValue)
                typeDescriptor.Add(retrivedType.Value);
            else
                typeDescriptor.Add(HappyType.Error);
            
            return typeDescriptor;
        }
    }
}