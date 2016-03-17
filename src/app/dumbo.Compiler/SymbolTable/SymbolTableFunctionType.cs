using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTableFunctionType : SymbolTableTypeEntry
    {
        public IList<HappyType> _parametertypes;
        public IList<HappyType> _returntypes;

        public SymbolTableFunctionType(IList<HappyType> parametertypes, IList<HappyType> returntypes)
        {
            _parametertypes = parametertypes;
            _returntypes = returntypes;
        }

        public IList<HappyType> parametertypes => _parametertypes;
        public IList<HappyType> returntypes => _returntypes;
    }
}
