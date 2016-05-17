using System;
using System.IO;

namespace dumbo.Compiler.CodeGenerator.LHZLib
{
    internal class LHZLibReader
    {
        public Module CreateLHZLIb()
        {
            Module output = new Module();
            string currentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CodeGenerator\LHZLib\LHZLib.c");
            StreamReader reader = new StreamReader(currentPath);
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
