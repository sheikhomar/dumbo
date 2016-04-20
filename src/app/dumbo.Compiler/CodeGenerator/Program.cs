using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CodeGenerator
{
    public class Program
    {
        private IList<Module> _moduleList;

        public Program()
        {
            _moduleList = new List<Module>();
        }

        public void AddModule(Module module)
        {
            _moduleList.Add(module);
        }

        public override string ToString()
        {
            string output = "";

            foreach (var item in _moduleList)
            {
                output += item;
            }

            return output;
        }
    }
}
