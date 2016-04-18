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
            var key = GetKey(name);
            _data.Add(key, new UndefinedValue());
        }

        public void Set(string name, Value value)
        {
            var key = GetKey(name);
            if (!_data.ContainsKey(key))
                throw new ArgumentOutOfRangeException(nameof(name), $"Variable '{name}' has not been allocated.");
            _data[key] = value;
        }

        public T Get<T>(string name) where T : Value
        {
            var key = GetKey(name);
            return _data[key] as T;
        }

        public bool Contains(string name)
        {
            var key = GetKey(name);
            return _data.ContainsKey(key);
        }

        private string GetKey(string name)
        {
            return name.ToLower();
        }
    }
}