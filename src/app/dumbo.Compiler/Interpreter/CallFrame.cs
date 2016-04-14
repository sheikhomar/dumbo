using System.Collections.Generic;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class CallFrame
    {
        private readonly Dictionary<string, Value> _data;

        public CallFrame(FuncDeclNode function)
        {
            Function = function;
            _data = new Dictionary<string, Value>();
        }

        public FuncDeclNode Function { get; set; }

        public void Allocate(string name, Value value)
        {
            _data.Add(name, value);
        }

        public Value GetValue(string name)
        {
            return _data[name];
        }
    }
}