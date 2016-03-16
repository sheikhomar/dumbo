using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.PrettyPrint
{
    public interface IPrettyPrinter
    {
        
        StringBuilder print(RootNode root);     //Create output for the pretty print tree

        void Append(string inpt);
        void EndLine(string inpt = "");
        void IndentIncrement();
        void IndentDecrement();
      
    }
}
