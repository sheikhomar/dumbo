using dumbo.Compiler;

namespace dumbo.WpfApp
{
    public class ProcessorResult
    {
        public EventReporter Reporter { get; }

        public ProcessorResult(EventReporter reporter)
        {
            Reporter = reporter;
        }
    }
}
