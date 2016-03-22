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

        public void AddVariableToSymbolTable(string name, HappyType Type, bool Unhideable, int Line, int Column)
        {
            var entry = SymbolTable.RetrieveSymbol(name);
            if (entry == null)
                SymbolTable.EnterSymbol(name, new SymbolTablePrimitiveType(Type), Unhideable);
            else if (entry.IsUnhideable == false)
            {
                if (entry.Depth == SymbolTable.Depth)
                    ErrorReporter.AddError(new CCError($"The variable {name} is already declared in this scope", Line, Column));
                else
                    SymbolTable.EnterSymbol(name, new SymbolTablePrimitiveType(Type), Unhideable);
            }
            else if (entry.IsUnhideable == true)
                ErrorReporter.AddError(new CCError($"The variable {name} can't override a unhideable variable", Line, Column));
        }
    }
}
