using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{ 
    [Serializable]
    public class IllegalHideException : Exception
    {
        public IllegalHideException() { }
        public IllegalHideException(string message) : base(message) { }
        public IllegalHideException(string message, Exception inner) : base(message, inner) { }
        protected IllegalHideException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
