using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    [Serializable]
    public class DuplicateDeclarationException : Exception
    {
        public DuplicateDeclarationException() { }
        public DuplicateDeclarationException(string message) : base(message) { }
        public DuplicateDeclarationException(string message, Exception inner) : base(message, inner) { }
        protected DuplicateDeclarationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
