using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.ScopeCheck
{
    public interface IScopeCheck
    {
        void ScopeCheck(ISymbolTable table);
    }
}
