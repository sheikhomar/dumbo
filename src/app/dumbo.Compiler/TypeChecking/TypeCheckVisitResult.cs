using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.TypeChecking
{
    public class TypeCheckVisitResult : VisitResult
    {
        private readonly List<TypeNode> _types = new List<TypeNode>();
        public IEnumerable<TypeNode> Types => _types;

        public TypeCheckVisitResult(bool isError)
        {

        }

        public TypeCheckVisitResult(TypeNode happyType)
        {
            _types.Add(happyType);
        }

        public TypeCheckVisitResult(IEnumerable<TypeNode> types)
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
