﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    internal class Stmt
    {
        public Stmt(string line)
        {
            Line = line;
        }

        public string Line { get; }
    }
}