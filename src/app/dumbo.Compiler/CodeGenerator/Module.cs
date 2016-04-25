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

        public string Print()
        {
            var builder = new StringBuilder();

            foreach (var item in _stmtList)
            {
                builder.AppendLine(item.Print());
                string pet = builder.ToString();
            }

            builder.AppendLine("");

            string hans = builder.ToString();

            return builder.ToString();
        }
    }
}
