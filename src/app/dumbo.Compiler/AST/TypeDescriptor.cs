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

        public IEnumerable<HappyType> Types => _types;
        public int GetNumberOfTypes() => _types.Count;
        public void Add(HappyType type) => _types.Add(type);
        public IList<HappyType> GetAsList() => new List<HappyType>();

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

        public void AddFirstToList(IList<HappyType> list)
        {
            list.Add(GetFirst());
        }

        public void AddTypesToList(IList<HappyType> list)
        {
            foreach (var type in _types)
            {
                list.Add(type);
            }
        }
    }
}
