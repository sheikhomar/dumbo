using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTableFunctionType : SymbolTableTypeEntry
    {
        public SymbolTableFunctionType()
        {
            parameters = new List<SymbolTableType>();
        }

        public List<SymbolTableType> parameters { get; set; }
    }
}
