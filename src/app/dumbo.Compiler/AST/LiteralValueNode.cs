using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class LiteralValueNode : ValueNode
    {
        public LiteralValueNode(string value, HappyType type, SourcePosition sourcePosition) : base(type)
        {
            Value = value;
            SourcePosition = sourcePosition;
        }

        public string Value { get; }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            return new TypeDescriptor(Type);
        }
        
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}