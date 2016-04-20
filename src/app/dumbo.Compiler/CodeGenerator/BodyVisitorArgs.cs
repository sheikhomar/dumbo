using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.CodeGenerator
{
    internal class BodyVisitorArgs : VisitorArgs
    {
        public BodyVisitorArgs(IList<HappyType> returnTypes)
        {
            ReturnTypes = returnTypes;
        }

        public IList<HappyType> ReturnTypes { get; } //todo, can currently be change.
    }
}
