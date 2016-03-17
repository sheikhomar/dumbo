using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTablePrimitiveType : SymbolTableTypeEntry
    {
        public SymbolTablePrimitiveType(HappyType type)
        {
            Type = type;
        }

        public HappyType Type { get; set; }
    }
}
