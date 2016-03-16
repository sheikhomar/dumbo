﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    class SymbolTableEntry
    {
        // These properties are used to identify an entry
        public string Name { get; set; }
        public SymbolTableType Type { get; set; }
        public int Depth { get; set; }

        // This property is used when closing an entire level
        public SymbolTableEntry OtherInLevel { get; set; }

        // This property references a previous entry with the same name as this entry
        public SymbolTableEntry OuterDecl { get; set; }
    }
}
