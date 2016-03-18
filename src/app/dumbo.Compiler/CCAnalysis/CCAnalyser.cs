using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.CCAnalysis
{
    public class CCAnalyser : ICCAnalyser
    {
        private ISymbolTable symbolTable;

        public CCAnalyser()
        {
            symbolTable = new SymbolTable.SymbolTable();
        }

        public ISymbolTable SymbolTable
        {
            get
            {
                return symbolTable;
            }
        }

        public bool IsEqual(HappyType inpt, HappyType inpt2)
        {
            return inpt.Equals(inpt2);
        }

        public bool IsListsEqual(IList<HappyType> inpt, IList<HappyType> inpt2)
        {
            //Comp each element and what?
            throw new NotFiniteNumberException();
        }
    }
}
