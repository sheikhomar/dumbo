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
        private readonly ISymbolTable _symbolTable;
        private readonly IErrorReporter _errorReporter;

        public CCAnalyser()
        {
            _symbolTable = new SymbolTable.SymbolTable();
            _errorReporter = new ErrorReporter();
        }

        public IErrorReporter ErrorReporter
        {
            get { return _errorReporter; }
        }

        public ISymbolTable SymbolTable
        {
            get { return _symbolTable; }
        }

        public bool IsEqual(HappyType inpt, HappyType inpt2)
        {
            return inpt.Equals(inpt2);
        }

        public bool IsListEqualToType(IList<HappyType> inpt, HappyType inpt2)
        {
            foreach (var element in inpt)
            {
                if (!element.Equals(inpt2))
                    return false;
            }
            return true;
        }

        public bool IsListsEqual(IList<HappyType> inpt, IList<HappyType> inpt2)
        {
            int i = 0;
            foreach (var element in inpt)
            {
                if (!element.Equals(inpt2[i]))
                    return false;
                i++;
            }
            return true;
        }

        public void AddToSymbolTable(string name)
        {
            //Hide symbol table? Only allow write via this method. Should still be possible to read
            ////Todo - check that symbol is not in table | gives exception atm
            //analyser.SymbolTable.RetrieveSymbol(param.Name);

            ////Does exists
            ////y  - is hidable
            ////y - is in this scope?
            ////y - I throw error         // x val is already declared in this scope | A
            ////n - I put my val.
            ////n - I throw error                 // x val is Global or parameter. | A
            ////n - alles ok

            ////Check if exists, then check unhidable - if not den we have no idea whether its in our scope or another...
            //analyser.SymbolTable.EnterSymbol(param.Name, new SymbolTablePrimitiveType(param.Type), true);
        }
    }
}
