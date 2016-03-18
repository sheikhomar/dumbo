using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    public interface ISymbolTable
    {
        void OpenScope();
        void CloseScope();

        void EnterSymbol(string name, SymbolTableTypeEntry type, bool unhideability = false);
        SymbolTableEntry RetrieveSymbol(string name);

        bool DeclaredLocally(string name);
    }
}
