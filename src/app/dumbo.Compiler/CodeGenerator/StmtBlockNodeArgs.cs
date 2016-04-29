using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    internal class StmtBlockNodeArgs : VisitorArgs
    {
        private bool _handled = false;

        public StmtBlockNodeArgs(IList<Stmt> prefix, IList<Stmt> suffix, VisitorArgs arg)
        {
            Prefix = prefix;
            Suffix = suffix;
            Arg = arg;
        }

        public IList<Stmt> Prefix { get; }
        public IList<Stmt> Suffix { get; }
        public VisitorArgs Arg { get; }
        public bool Handled
        {
            get
            {
                return _handled;
            }
            set
            {
                if (_handled)
                    throw new Exception("Cannot be handled twice");
                else
                    _handled = value;
            }
        }
    }
}
