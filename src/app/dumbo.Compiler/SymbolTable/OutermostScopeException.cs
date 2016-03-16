using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    [Serializable]
    public class OutermostScopeException : Exception
    {
        public OutermostScopeException() { }
        public OutermostScopeException(string message) : base(message) { }
        public OutermostScopeException(string message, Exception inner) : base(message, inner) { }
        protected OutermostScopeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
