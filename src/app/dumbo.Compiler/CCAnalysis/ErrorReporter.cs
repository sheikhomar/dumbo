using System;
using System.Collections.Generic;

namespace dumbo.Compiler.CCAnalysis
{
    public class ErrorReporter : IErrorReporter
    {
        public ErrorReporter()
        {
            Errors = new List<CCError>();
        }

        public void AddError(CCError error)
        {
            Errors.Add(error);
        }

        public IList<CCError> Errors { get; }

        public void AddError(string error)
        {
            Errors.Add(new CCError(error, 0, 0));
        }
    }
}