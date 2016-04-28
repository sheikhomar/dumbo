using System.Text;

namespace dumbo.Tests.SemanticAnalysis
{
    internal class ProgramFragment
    {
        public ProgramFragment()
        {
            Buffer = new StringBuilder();
        }

        public int Line { get; set; }
        public string Text { get; set; }

        public StringBuilder Buffer { get; }
    }
}