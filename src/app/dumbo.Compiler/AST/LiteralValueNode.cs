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

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}