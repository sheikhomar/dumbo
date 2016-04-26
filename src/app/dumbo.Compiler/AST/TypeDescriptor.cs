using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class TypeDescriptor
    {
        private readonly List<TypeNode> _types;

        private TypeDescriptor()
        {
            _types = new List<TypeNode>();
        }

        public TypeDescriptor(TypeNode type) : this()
        {
            _types.Add(type);
        }

        public TypeDescriptor(IEnumerable<TypeNode> types) : this()
        {
            _types.AddRange(types);
        }

        public T GetFirstAs<T>() where T : TypeNode
        {
            return _types[0] as T;
        }

        public T GetAs<T>(int index) where T : TypeNode
        {
            return _types[index] as T;
        }

        public bool MultiType => _types.Count > 1;

        public int Count => _types.Count;
    }
}