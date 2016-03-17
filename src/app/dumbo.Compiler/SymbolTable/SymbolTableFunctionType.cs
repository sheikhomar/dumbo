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
            parameters = new List<HappyType>();
        }

        public List<HappyType> parameters { get; set; }
    }
}
