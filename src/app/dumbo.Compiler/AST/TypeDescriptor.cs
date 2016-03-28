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

        public TypeDescriptor(IList<HappyType> happyTypeList)
        {
            _types = new List<HappyType>();
            foreach (var type in happyTypeList)
            {
                _types.Add(type);
            }
        }

        public IEnumerable<HappyType> Types => _types;
        public int GetNumberOfTypes() => _types.Count;
        public void Add(HappyType type) => _types.Add(type);

        public HappyType GetFirst()
        {
            if  (_types.Any())
                return _types[0];
            throw new IndexOutOfRangeException("Type descriptor is empty.");
        }

        public HappyType GetAt(int index)
        {
            if (index >= _types.Count)
                throw new IndexOutOfRangeException("No type at index " + index);
            return _types[index];
        }
    }
}
