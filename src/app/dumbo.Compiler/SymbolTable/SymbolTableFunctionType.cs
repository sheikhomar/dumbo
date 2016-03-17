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
        public SymbolTableFunctionType()
        {
            parametertypes = new List<HappyType>();
            returntypes = new List<HappyType>();
        }

        public List<HappyType> parametertypes { get; set; }
        public List<HappyType> returntypes { get; set; }
    }
}
