using System.IO;
using System.Linq;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.SyntaxAnalysis;
using NUnit.Framework;

namespace dumbo.Tests.SyntaxAnalysis
{
    [TestFixture]
    public class GeneralParserTests
    {
        [Test]
        public void TestCodeSnippet()
        {
            var parser = new Parser(Utils.GetGrammarTablePath());

            var dir = Path.GetDirectoryName(GetType().Assembly.Location);
            var path = Path.Combine(dir, "ParserTest.txt");
            using (var fs = new StreamReader(path))
            {
                StringBuilder programBuilder = new StringBuilder();
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    if (" //New Program ".Equals(line))
                    {
                        var programText = programBuilder.ToString();
                        var result = parser.Parse(new StringReader(programText));
                        if (result.Errors.Any())
                        {


                            Assert.Fail($"Parser error for program: \n\r\n\r{programText}");
                        }
                        
                        programBuilder = new StringBuilder();
                    }
                    else
                    {
                        programBuilder.AppendLine(line);
                    }
                }
            }
        }
    }
}