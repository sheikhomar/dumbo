using System;
using System.Collections.Generic;
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
    }
}
