using dumbo.Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dumbo.Compiler.CCAnalysis
{
    public interface ICCAnalysable
    {
        void CCAnalyse(ICCAnalyser analyser);

    }
}
