using System.Collections.Generic;

namespace dumbo.Compiler.CCAnalysis
{
    public interface IErrorReporter
    {
        void AddError(string error);
        void AddError(CCError error);
        IList<CCError> Errors { get; }
    }
}