using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.AST
{
    public class TypeDescriptor
    {
        private IList<HappyType> _types;

        public TypeDescriptor()
        {
            _types = new List<HappyType>();
        }

        public IList<HappyType> Types { get; }

        public void Add(HappyType type)
        {
            _types.Add(type);
        }

        public HappyType GetFirst() => _types[0];
    }
}
