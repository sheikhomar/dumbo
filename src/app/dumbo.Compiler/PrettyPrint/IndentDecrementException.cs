using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.PrettyPrint
{
    public class IndentDecrementException : Exception
    {
        public IndentDecrementException()
        {

        }

        public IndentDecrementException(string message) : base(message)
        {

        }

        public IndentDecrementException(string message, Exception inner) : base(message,inner)
        {

        }

    }
}
