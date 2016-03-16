using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    interface ISymbolTable
    {
        void OpenScope();
        void CloseScope();

        void EnterSymbol(string name, SymbolTableTypeEntry type);
        SymbolTableEntry RetrieveSymbol(string name);

        bool DeclaredLocally(string name);
    }
}
