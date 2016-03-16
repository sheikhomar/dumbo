using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    class SymbolTable : ISymbolTable
    {
        public SymbolTable()
        {
            Depth = 0;
            Table = new Dictionary<string, SymbolTableEntry>();
            DepthContent = new List<string>();
            DepthContentStack = new Stack<List<string>>();
        }

        private Dictionary<string, SymbolTableEntry> Table { get; set; }
        private int Depth { get; set; }
        private List<string> DepthContent { get; set; }
        private Stack<List<string>> DepthContentStack { get; set; }

        #region interface methods
        public void OpenScope()
        {
            Depth++;
            DepthContentStack.Push(DepthContent);
            DepthContent = new List<string>();
        }

        public void CloseScope()
        {
            throw new NotImplementedException();
        }

        public bool DeclaredLocally(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteSymbol(string name)
        {
            throw new NotImplementedException();
        }

        public void EnterSymbol(string name, SymbolTableType type)
        {
            SymbolTableEntry oldEntry = Table[name];
            if (oldEntry != null && oldEntry.Depth == Depth)
            {
                throw new Exception();
            }
            
            throw new NotImplementedException();
        }

        public SymbolTableEntry RetrieveSymbol(string name)
        {
            SymbolTableEntry retrieved = Table[name];

            return retrieved;
        }
        #endregion
    }
}
