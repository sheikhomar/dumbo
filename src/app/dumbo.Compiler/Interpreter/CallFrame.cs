using System;
using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class CallFrame
    {
        private readonly Stack<BlockFrame> _blockFrames;

        public CallFrame(FuncDeclNode function)
        {
            Function = function;
            _blockFrames = new Stack<BlockFrame>();
            _blockFrames.Push(new BlockFrame());
        }

        public BlockFrame CurrentBlockFrame => _blockFrames.Peek();
        public FuncDeclNode Function { get; }

        public void EnterBlock()
        {
            _blockFrames.Push(new BlockFrame());
        }

        public void ExitBlock()
        {
            _blockFrames.Pop();
        }

        public void Set(string name, Value value)
        {
            bool foundItem = false;
            foreach (var blockFrame in _blockFrames)
            {
                if (blockFrame.Contains(name))
                {
                    blockFrame.Set(name, value);
                    foundItem = true;
                    break;
                }
            }
            if (!foundItem)
                throw new ArgumentOutOfRangeException(nameof(name), $"Variable '{name}' has not been allocated.");
        }

        public T Get<T>(string name) where T : Value
        {
            foreach (var blockFrame in _blockFrames)
            {
                if (blockFrame.Contains(name))
                {
                    return blockFrame.Get<T>(name);
                }
            }
            throw new ArgumentOutOfRangeException(nameof(name), $"Variable '{name}' has not been allocated.");
        }
    }
}
