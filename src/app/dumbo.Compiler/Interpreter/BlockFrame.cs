using System;
using System.Collections.Generic;

namespace dumbo.Compiler.Interpreter
{
    public class BlockFrame
    {
        private readonly Dictionary<string, Value> _data;

        public BlockFrame()
        {
            _data = new Dictionary<string, Value>();   
        }

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

        public bool Contains(string name)
        {
            return _data.ContainsKey(name);
        }
    }
}