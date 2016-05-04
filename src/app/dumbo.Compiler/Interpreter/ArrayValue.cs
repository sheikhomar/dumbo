using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    class ArrayValue : Value
    {
        public TypeNode Type { get; }
        private Value[] InternalArray;

        public ArrayValue(TypeNode type, ExpressionListNode sizes)
        {
            Type = type;
        }
    }
}
