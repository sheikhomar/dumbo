using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class Event
    {
        public Event(EventKind kind, string message, SourcePosition sourcePosition)
        {
            Kind = kind;
            Message = message;
            SourcePosition = sourcePosition;
        }

        public EventKind Kind { get; }
        public string Message { get; }
        public SourcePosition SourcePosition { get; }
    }
}
