﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTableEntry
    {
        public SymbolTableEntry(string name, SymbolTableTypeEntry type, int depth, SymbolTableEntry outerDecl)
        {
            Name = name.ToLower();
            Type = type;
            Depth = depth;
            OuterDecl = outerDecl;
        }

        // These properties are used to identify an entry
        public string Name { get; set; }
        public SymbolTableTypeEntry Type { get; set; }
        public int Depth { get; set; }

        // This property references a previous entry with the same name as this entry
        public SymbolTableEntry OuterDecl { get; set; }
    }
}