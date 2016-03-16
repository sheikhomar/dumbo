using System.IO;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.SyntaxAnalysis;

namespace dumbo.Tests.SyntaxAnalysis
{
    public class BaseParserTester
    {
        protected RootNode ParseAndGetRootNode(string programText)
        {
            var parser = new Parser(Utils.GetGrammarTablePath());
            var result = parser.Parse(new StringReader(programText));
            return result.Root;
        }
    }
}