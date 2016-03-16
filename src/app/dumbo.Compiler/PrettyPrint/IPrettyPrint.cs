using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.PrettyPrint
{
    public interface IPrettyPrint
    {
        void PrettyPrint(IPrettyPrinter strBuilder);
    }
}
