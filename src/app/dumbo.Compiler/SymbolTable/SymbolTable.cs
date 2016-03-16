using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.SymbolTable
{
    public class SymbolTable : ISymbolTable
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
            if (Depth == 0)
                throw new OutermostScopeException("The outermost scope cannot be closed - programmer error");

            List<SymbolTableEntry> restoreTable = new List<SymbolTableEntry>();

            foreach (string closingName in DepthContent)
            {
                SymbolTableEntry entry = RetrieveSymbol(closingName);
                SymbolTableEntry outerEntry = entry.OuterDecl;
                Table.Remove(closingName);

                if (outerEntry != null)
                    restoreTable.Add(outerEntry);
            }

            Depth--;
            DepthContent = DepthContentStack.Pop();

            foreach (SymbolTableEntry entry in restoreTable)
            {
                EnterSymbol(entry.Name, entry.Type);
            }
        }

        public bool DeclaredLocally(string name)
        {
            SymbolTableEntry entry = RetrieveSymbol(name);
            return (entry.Depth == Depth);
        }

        public void EnterSymbol(string name, SymbolTableTypeEntry type)
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

            DepthContent.Add(name.ToLower());

            if (oldEntry == null)
            {
                Table.Add(newEntry.Name, newEntry);
            }
            else //This is the case when the entry already exist at an outer level
            {
                Table.Remove(name.ToLower());
                newEntry.OuterDecl = oldEntry;
                Table.Add(newEntry.Name, newEntry);
            }
        }

        public SymbolTableEntry RetrieveSymbol(string name)
        {
            if (Table.ContainsKey(name.ToLower()))
                return Table[name.ToLower()];
            else
                return null;
        }
        #endregion
    }
}
