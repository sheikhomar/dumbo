using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.PrettyPrint
{
    public class PrettyPrinter : IPrettyPrinter
    {
        private StringBuilder strBuilder;
        private int indentSize;
        private bool inLine;

        //Public methods

        public StringBuilder print(RootNode root)
        {
            strBuilder = new StringBuilder();
            inLine = false;
            indentSize = 0;


            root.PrettyPrint(this);

            return strBuilder;
        }

        public void Append(string inpt)
        {
            if (inLine)
            {
                Add(inpt);
            }
            else
            {
                Indent();
                Add(inpt);
            }
        }

        public void EndLine(string inpt = "")
        {
            AddLine(inpt);
            inLine = false;
        }

        public void IndentIncrement()
        {
            indentSize++;
        }

        public void IndentDecrement()
        {
            if (indentSize >= 1)
                indentSize--;
            else
                throw new IndentDecrementException("Unable to decrement as indentSize is less than 1");
        }

        //Private methods
        private void Add(string inpt)
        {
            strBuilder.Append(inpt);
        }

        private void AddLine(string inpt)
        {
            strBuilder.AppendLine(inpt);
        }

        private void Indent()
        {
            inLine = true;
            for (int i = 0; i < indentSize; i++)
            {
                strBuilder.Append("  ");
            }
        }
    }
}
