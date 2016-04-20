using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    public class Stmt
    {
        public Stmt(string line)
        {
            Line = line;
        }

        public string Line { get; private set; }

        public void Append(string linePart)
        {
            Line += linePart;
        }
    }
}
