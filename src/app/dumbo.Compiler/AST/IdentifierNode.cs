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

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            HappyType? retrivedType = null;
            var symbolEntry = symbolTable.RetrieveSymbol(Name);
            if (symbolEntry != null)
            {
                retrivedType = (symbolEntry.Type as SymbolTablePrimitiveType)?.Type;
            }

            if (retrivedType.HasValue)
                return new TypeDescriptor(retrivedType.Value);
            else
                return new TypeDescriptor(HappyType.Error);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {

        }
    }
}