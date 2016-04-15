using System;
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

        public void Allocate(string name)
        {
            _data.Add(name, new UndefinedValue());
        }

        public void Set(string name, Value value)
        {
            if (!_data.ContainsKey(name))
                throw new ArgumentOutOfRangeException(nameof(name), $"Variable '{name}' has not been allocated.");
            _data[name] = value;
        }
        
        public T Get<T>(string name) where T : Value
        {
            return _data[name] as T;
        }
    }
}