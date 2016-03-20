using System.Collections.Generic;

namespace dumbo.Compiler.CCAnalysis
{
    public interface IErrorReporter
    {
        void AddError(string error);
        IList<string> Errors { get; }
    }
}