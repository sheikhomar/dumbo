using System;
using System.Collections.Generic;

namespace dumbo.Compiler.CCAnalysis
{
    internal class ErrorReporter : IErrorReporter
    {
        public ErrorReporter()
        {
            Errors = new List<string>();
        }

        public IList<string> Errors { get; }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}