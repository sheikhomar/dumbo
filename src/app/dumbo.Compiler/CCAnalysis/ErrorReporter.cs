using System;
using System.Collections.Generic;

namespace dumbo.Compiler.CCAnalysis
{
    internal class ErrorReporter : IErrorReporter
    {
        private IList<string> errors;

        public ErrorReporter()
        {
            errors = new List<string>();
        }

        public void AddError(string error)
        {
            errors.Add(error);
        }
    }
}