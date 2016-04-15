using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    internal class FuncVisitorArgs : VisitorArgs
    {
        public FuncVisitorArgs(bool visitBody)
        {
            VisitBody = visitBody;
        }

        public bool VisitBody { get; }
    }
}
