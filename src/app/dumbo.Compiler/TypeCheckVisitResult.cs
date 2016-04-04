using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class TypeCheckVisitResult : VisitResult
    {
        private readonly List<HappyType> _types = new List<HappyType>();
        public IEnumerable<HappyType> Types => _types;

        public TypeCheckVisitResult(HappyType happyType)
        {
            _types.Add(happyType);
        }

        public TypeCheckVisitResult(IEnumerable<HappyType> types)
        {
            _types.AddRange(types);
        }

        public bool Equals(TypeCheckVisitResult other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;
            
            return Types.SequenceEqual(other.Types);
        }
    }
}