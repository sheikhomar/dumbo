using System.Text;

namespace dumbo.Tests
{
    public class TestProgramFragment
    {
        public TestProgramFragment(int line)
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
