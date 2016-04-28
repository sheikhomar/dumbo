using System;

namespace dumbo.Compiler.AST
{
    public class ConstDeclNode : BaseNode
    {
        public string Name { get; }
        public TypeNode Node { get; }
        public ValueNode Value { get; }

        public ConstDeclNode(string name, TypeNode typeNode, ValueNode valueNode, SourcePosition srcPos)
        {
            Name = name;
            Node = typeNode;
            Value = valueNode;
            SourcePosition = srcPos;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}