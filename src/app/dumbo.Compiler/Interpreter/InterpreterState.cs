using System.Collections.Generic;

namespace dumbo.Compiler.Interpreter
{
    public class InterpreterState
    {
        protected IList<Value> _data;
        protected InterpreterStatus _status;

        public InterpreterState()
        {
            _data = new List<Value>();
            _status = InterpreterStatus.Stopped;
        }
    }
}