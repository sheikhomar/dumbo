using System.Collections.Generic;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.SyntaxAnalysis;
using System.IO;
using dumbo.Compiler.AST;

namespace dumbo.Tests
{
    public abstract class BaseTest
    {
        protected ParserResult Parse(string programText)
        {
            var parser = new Parser(Utils.GetGrammarTablePath());
            return parser.Parse(new StringReader(programText));
        }


        protected RootNode ParseAndGetRootNode(string programText)
        {
            return Parse(programText).Root;
        }

        protected static void AppendProgram(string fileName, int programLine, string programText, StringBuilder buffer)
        {
            buffer.Append("The program in question can be found in ");
            buffer.Append(fileName);
            buffer.Append(" line ");
            buffer.Append(programLine);
            buffer.AppendLine(":");
            buffer.AppendLine(programText);
        }


    }
}