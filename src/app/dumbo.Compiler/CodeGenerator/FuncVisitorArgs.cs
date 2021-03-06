﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    internal class FuncVisitorArgs : VisitorArgs
    {
        public FuncVisitorArgs(bool visitBody, VisitorArgs arg)
        {
            VisitBody = visitBody;
            Arg = arg;
        }

        public bool VisitBody { get; }
        public VisitorArgs Arg { get; }
    }
}
