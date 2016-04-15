using System;
using System.Collections;
using System.Collections.Generic;

namespace dumbo.Compiler.Interpreter
{
    public class ReturnFromFunctionException : Exception
    {
        public ReturnValue ReturnValue { get; }

        public ReturnFromFunctionException(ReturnValue returnValue)
        {
            ReturnValue = returnValue;
        }
    }
}