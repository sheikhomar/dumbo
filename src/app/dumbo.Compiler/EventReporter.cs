using System.Collections.Generic;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class EventReporter : IEventReporter
    {
        private readonly IList<Event> _events;

        public EventReporter()
        {
            _events = new List<Event>();
        } 

        public void Error(string message, SourcePosition sourcePosition)
        {
            _events.Add(new Event(EventKind.Error, message, sourcePosition));
        }

        public void Warn(string message, SourcePosition sourcePosition)
        {
            _events.Add(new Event(EventKind.Warning, message, sourcePosition));
        }

        public IEnumerable<Event> GetEvents()
        {
            return _events;
        }
    }
}