using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    internal class StmtBlockNodeArgs : VisitorArgs
    {
        public StmtBlockNodeArgs(IList<Stmt> prefix, IList<Stmt> suffix)
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        public IList<Stmt> Prefix { get; }
        public IList<Stmt> Suffix { get; }
    }
}
