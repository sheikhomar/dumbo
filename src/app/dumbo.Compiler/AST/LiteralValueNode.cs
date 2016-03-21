using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class LiteralValueNode : ValueNode
    {
        public LiteralValueNode(string value, HappyType type) : base(type)
        {
            Value = value;
        }

        public string Value { get; }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            return new TypeDescriptor(Type);
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Value);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            
        }
    }
}