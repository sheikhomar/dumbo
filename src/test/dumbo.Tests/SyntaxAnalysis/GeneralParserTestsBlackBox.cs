using System.IO;
using System.Linq;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.SyntaxAnalysis;
using NUnit.Framework;

namespace dumbo.Tests.SyntaxAnalysis
{
    [TestFixture]
    public class GeneralParserTestsBlackBox
    {
        [Test]
        public void TestCodeSnippetParser()
        {
            var parser = new Parser(Utils.GetGrammarTablePath());

            var dir = Path.GetDirectoryName(GetType().Assembly.Location);
            var path = Path.Combine(dir, "ParserTests.txt");
            using (var fs = new StreamReader(path))
            {
                StringBuilder programBuilder = new StringBuilder();
                string line = fs.ReadLine();
                int counter = 0;
                int nextProgramLine = 0;
                while (line != null)
                {
                    if (" //New Program ".Equals(line))
                    {
                        var programText = AppendLines(ref line, fs, ref counter).ToString();
                        var result = parser.Parse(new StringReader(programText));
                        if (result.Errors.Any())
                        {
                            var errorMessages = result.Errors.Select(e => e.GetErrorMessage()).ToArray();

                            Assert.Fail($"Parser error for program at line {nextProgramLine}: \n\r \n\r{string.Join(", ", errorMessages)} \n\r{programText}");
                        }

                        nextProgramLine = counter + 2;
                    }
                    else
                    {
                        line = fs.ReadLine();
                    }
                    counter++;
                }
            }
        }

        private StringBuilder AppendLines(ref string line, StreamReader fs, ref int counter)
        {
            StringBuilder builder = new StringBuilder();
            line = fs.ReadLine();

            while (!" //New Program ".Equals(line) && !" //New Program Failing".Equals(line))
            {
                builder.AppendLine(line);
                line = fs.ReadLine();
                counter++;
                if (line == null)
                {
                    break;
                }
            }

            return builder;
        }
    }
}
