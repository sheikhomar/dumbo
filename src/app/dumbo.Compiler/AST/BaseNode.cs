using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    abstract public class BaseNode : IPrettyPrint
    {
        virtual public void PrettyPrint(StringBuilder StrBuilder)
        {
            StrBuilder.Append("***" + this.ToString() + "***" + "\n");
        }
    }
}