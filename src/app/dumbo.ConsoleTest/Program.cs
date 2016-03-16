using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.SymbolTable;

namespace dumbo.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolTable table = new SymbolTable();

            table.EnterSymbol("fisk", SymbolTableType.Text);
            table.EnterSymbol("Omar", SymbolTableType.Number);
            table.OpenScope();
            table.EnterSymbol("Marc", SymbolTableType.Boolean);
            Console.WriteLine(table.RetrieveSymbol("maRC").Name);
            table.CloseScope();
            if (null == table.RetrieveSymbol("Marc"))
                Console.WriteLine("Success");

            Console.WriteLine("Det kan bare arbejd'");
            Console.ReadKey(true);
        }
    }
}
