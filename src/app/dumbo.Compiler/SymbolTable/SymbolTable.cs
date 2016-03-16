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
            foreach (string closingName in DepthContent)
            {
                SymbolTableEntry entry = RetrieveSymbol(closingName);
                SymbolTableEntry outerEntry = entry.OuterDecl;
                Table.Remove(closingName);

                if (outerEntry != null)
                    EnterSymbol(outerEntry.Name, outerEntry.Type);
            }

            Depth--;
            DepthContent = DepthContentStack.Pop();
        }

        public bool DeclaredLocally(string name)
        {
            SymbolTableEntry entry = RetrieveSymbol(name);
            return (entry.Depth == Depth);
        }

        public void EnterSymbol(string name, SymbolTableType type)
        {
            SymbolTableEntry oldEntry = RetrieveSymbol(name);
            if (oldEntry != null && oldEntry.Depth >= Depth)
            {
                if (oldEntry.Depth > Depth)
                    throw new ArgumentException("oldEntry was deeper than current entry - programmer error");
                else
                    throw new DuplicateDeclarationException(string.Format("Error: Multiple declarations of {0} exist in this scope", name));
            }

            SymbolTableEntry newEntry = new SymbolTableEntry(name, type, Depth, null);

            DepthContent.Add(name);

            if (oldEntry == null)
            {
                Table.Add(newEntry.Name, newEntry);
            }
            else //This is the case when the entry already exist at an outer level
            {
                Table.Remove(name);
                newEntry.OuterDecl = oldEntry;
                Table.Add(newEntry.Name, newEntry);
            }
        }

        public SymbolTableEntry RetrieveSymbol(string name)
        {
            SymbolTableEntry retrieved = Table[name];

            return retrieved;
        }
        #endregion
    }
}
