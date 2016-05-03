using System.Text;

namespace dumbo.Tests.SemanticAnalysis
{
    public class ProgramFragment
    {
        public ProgramFragment(int line)
        {
            Buffer = new StringBuilder();
            Line = line;
        }

        public int Line { get; }
        public string Text => Buffer.ToString();
        public bool ShouldFail { get; set; }
        public bool ShouldPass => !ShouldFail;

        public StringBuilder Buffer { get; }
        
    }
}
