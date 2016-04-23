using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.SyntaxAnalysis;
using NUnit.Framework;

namespace dumbo.Tests
{
    [TestFixture]
    public class SemanticBlackBoxTests
    {
        [Test]
        public void SemanticVisitor()
        {
            var parser = new Parser(Utils.GetGrammarTablePath());

            var dir = Path.GetDirectoryName(GetType().Assembly.Location);
            var path = Path.Combine(dir, "SemanticTests.txt");
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
                        else
                        {
                            var errors = RunSemanticAnalysis(result.Root).ToList();
                            if (errors.Any())
                            {
                                var errorMessages = errors.Select(e => e.Message).ToArray();

                                Assert.Fail($"CCAnalyser error for program at line {nextProgramLine}: \n\r \n\r{string.Join(", ", errorMessages)} \n\r{programText}");
                            }
                        }
                        
                        nextProgramLine = counter + 2;
                    }
                    else if (" //New Program Failing".Equals(line))
                    {
                        var programText = AppendLines(ref line, fs, ref counter).ToString();
                        var result = parser.Parse(new StringReader(programText));
                        if (!result.Errors.Any())
                        {
                            var errors = RunSemanticAnalysis(result.Root).ToList();
                            if (!errors.Any())
                            {
                                var errorMessages = errors.Select(e => e.Message).ToArray();

                                Assert.Fail($"Error! No error found. The code is supposed to fail at line {nextProgramLine}:\n\r{programText}");
                            }
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

        private IEnumerable<Event> RunSemanticAnalysis(RootNode root)
        {
            foreach (var func in BuiltInFuncDeclNode.GetBuiltInFunctions())
                root.FuncDecls.Add(func);

            var reporter = new EventReporter();
            var scopeChecker = new ScopeCheckVisitor(reporter);
            var typeChecker = new TypeCheckVisitor(reporter);
            root.Accept(scopeChecker, new VisitorArgs());
            root.Accept(typeChecker, new VisitorArgs());
            return reporter.GetEvents().Where(e => e.Kind == EventKind.Error);
        }
    }
}