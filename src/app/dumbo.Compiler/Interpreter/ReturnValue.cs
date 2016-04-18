using System.Collections.Generic;

namespace dumbo.Compiler.Interpreter
{
    public class ReturnValue : Value
    {
        public List<Value> ReturnValues { get; }

        public ReturnValue()
        {
            ReturnValues = new List<Value>();
        }

        public ReturnValue(IEnumerable<Value> values) : this()
        {
            ReturnValues.AddRange(values);
        }
    }
}