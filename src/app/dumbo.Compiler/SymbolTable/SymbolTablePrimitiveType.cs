using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTablePrimitiveType : SymbolTableTypeEntry
    {
        public SymbolTablePrimitiveType(SymbolTableType type)
        {
            Type = type;
        }

        public SymbolTableType Type { get; set; }
    }
}
