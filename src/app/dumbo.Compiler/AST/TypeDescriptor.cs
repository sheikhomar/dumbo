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

        public TypeDescriptor(HappyType happyType)
        {
            _types = new List<HappyType>();
            Add(happyType);
        }

        public IList<HappyType> Types => _types;

        public void Add(HappyType type)
        {
            _types.Add(type);
        }

        public HappyType GetFirst()
        {
            if  (_types.Any())
                return _types[0];
            throw new IndexOutOfRangeException("Type descriptor is empty.");
        }
    }
}
