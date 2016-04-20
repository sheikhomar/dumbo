using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    public class Stmt
    {
        private string _line;

        public Stmt(string line)
        {
            _line = line;
        }

        public void Append(string linePart)
        {
            _line += linePart;
        }

        public string Print()
        {
            return _line;
        }
    }
}
