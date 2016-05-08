using System;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.TypeChecking
{
    public class ErrorTypeNode : TypeNode, IEquatable<ErrorTypeNode>
    {
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            throw new InvalidOperationException("Only made for the sake of type checking");
        }

        public bool Equals(ErrorTypeNode other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ErrorTypeNode);
        }

        public override string ToString()
        {
            return "Error";
        }
    }
}