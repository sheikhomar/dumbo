using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public interface IEventReporter
    {
        void Error(string message, SourcePosition sourcePosition);
        void Warn(string message, SourcePosition sourcePosition);
    }
}