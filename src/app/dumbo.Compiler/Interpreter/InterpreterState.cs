using System.Collections.Generic;

namespace dumbo.Compiler.Interpreter
{
    public class InterpreterState
    {
        protected InterpreterStatus _status;

        public InterpreterState()
        {
            _status = InterpreterStatus.Stopped;
        }
    }
}