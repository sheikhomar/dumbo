using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    public class Module
    {
        private IList<Stmt> _stmtList;

        public Module()
        {
            _stmtList = new List<Stmt>();
        }

        public void Append(Stmt stmt)
        {
            _stmtList.Add(stmt);
        }

        public override string ToString()
        {
            string output = "";

            foreach (var item in _stmtList)
            {
                output += $"{item}\n";
            }

            return output;
        }
    }
}
