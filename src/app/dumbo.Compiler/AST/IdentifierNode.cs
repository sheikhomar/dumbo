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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            var hans = analyser.SymbolTable.RetrieveSymbol(Name); //adde exception? otherwise its null.

            //Check if null, if so, then error message...

            
        }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            var typeDescriptor = new TypeDescriptor();
            var retrivedType = (symbolTable.RetrieveSymbol(Name).Type as SymbolTablePrimitiveType)?.Type;

            if (retrivedType.HasValue)
                typeDescriptor.Add(retrivedType.Value);
            else
                typeDescriptor.Add(HappyType.Error);
            
            return typeDescriptor;
        }
    }
}