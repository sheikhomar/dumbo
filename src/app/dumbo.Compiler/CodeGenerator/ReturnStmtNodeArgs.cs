using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.CodeGenerator
{
    internal class ReturnStmtNodeArgs : VisitorArgs
    {
        public ReturnStmtNodeArgs(IList<ArrayTypeNode> arrTypeNode, VisitorArgs arg)
        {
            ArrTypeNode = arrTypeNode;
            Arg = arg;
        }

        public IList<ArrayTypeNode> ArrTypeNode { get; }
        public VisitorArgs Arg { get; }
    }
}
