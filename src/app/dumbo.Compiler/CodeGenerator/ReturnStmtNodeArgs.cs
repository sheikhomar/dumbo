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
        public ReturnStmtNodeArgs(IList<TypeNode> arrTypeNode, VisitorArgs arg)
        {
            ArrTypeNode = arrTypeNode;
            Arg = arg;
        }

        public IList<TypeNode> ArrTypeNode { get; }
        public VisitorArgs Arg { get; }
    }
}
