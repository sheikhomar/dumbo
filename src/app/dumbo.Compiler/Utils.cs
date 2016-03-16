﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler
{
    public class Utils
    {
        public static string GetGrammarTablePath()
        {
            string appDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string baseDir = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\..\\..\\"));
            string grammarPath = Path.Combine(baseDir, "Grammar");
            string path = Path.Combine(grammarPath, "HappyZ-Grammar.egt");
            return path;
        }
    }
}