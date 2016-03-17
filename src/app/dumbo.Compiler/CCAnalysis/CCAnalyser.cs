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








        //Symbol tabel parse
        public void CloseScope()
        {
            symbolTable.CloseScope();
        }

        public bool DeclaredLocally(string name)
        {
            return symbolTable.DeclaredLocally(name);
        }

        public void EnterSymbol(string name, SymbolTableTypeEntry type)
        {
            symbolTable.EnterSymbol(name, type);
        }

        public void OpenScope()
        {
            symbolTable.OpenScope();
        }

        public SymbolTableEntry RetrieveSymbol(string name)
        {
            return symbolTable.RetrieveSymbol(name);
        }
    }
}
