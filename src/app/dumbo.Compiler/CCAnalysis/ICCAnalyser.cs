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

    }
}
