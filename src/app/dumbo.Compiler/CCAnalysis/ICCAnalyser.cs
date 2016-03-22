using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.SymbolTable;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.CCAnalysis
{
    public interface ICCAnalyser
    {
        ISymbolTable SymbolTable{ get; }
        IErrorReporter ErrorReporter { get; }
        bool IsListsEqual(IList<HappyType> inpt, IList<HappyType> inpt2);
        bool IsListEqualToType(IList<HappyType> inpt, HappyType inpt2);
        bool IsEqual(HappyType inpt, HappyType inpt2);
        void AddVariableToSymbolTable(string name, HappyType Type, bool Unhideable, int Line, int Column);

    }
}
