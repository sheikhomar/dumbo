using System;

namespace dumbo.Compiler.AST
{
    public class ConstDeclNode : BaseNode , IVariableDeclNode
    {
        public string Name { get; }
        public TypeNode Type { get; }
        public ValueNode Value { get; }

        public ConstDeclNode(string name, TypeNode typeNode, ValueNode valueNode, SourcePosition srcPos)
        {
            Name = name;
            Type = typeNode;
            Value = valueNode;
            SourcePosition = srcPos;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}