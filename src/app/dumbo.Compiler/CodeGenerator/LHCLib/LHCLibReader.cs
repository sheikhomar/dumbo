using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator.LHCLib
{
    internal class LHCLibReader
    {
        public Module CreateLHCLIb()
        {
            Module output = new Module();
            StreamReader reader = new StreamReader("LHCLib.c");
            string currentLine = reader.ReadLine();

            while (currentLine != null)
            {
                output.Append(new Stmt(currentLine));
                currentLine = reader.ReadLine();
            }

            return output;
        }
    }
}
