using System;
using System.Text;
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
            var typeDescriptor = new TypeDescriptor();
            typeDescriptor.Add(this.Type);

            return typeDescriptor;
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Value);
        }
    }
}