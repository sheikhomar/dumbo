using System;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    public class InterpretationErrorException : Exception
    {
        public BaseNode Node { get; }

        public InterpretationErrorException(string message, BaseNode node) : base(message)
        {
            Node = node;
        }
    }
}