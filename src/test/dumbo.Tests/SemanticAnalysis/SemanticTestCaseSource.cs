using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.SyntaxAnalysis;
using dumbo.Compiler.TypeChecking;
using NUnit.Framework;

namespace dumbo.Tests.SemanticAnalysis
{
    [TestFixture]
    public class SemanticTests
    {
        [Test, TestCaseSource(typeof(SemanticSuccessfullTestCaseFactory),
            nameof(SemanticSuccessfullTestCaseFactory.TestCases))]
        public void TestProgram(int startLine, string programText)
        {
            var rootNode = Parse(startLine, programText);
            var errors = RunSemanticAnalysis(rootNode);
            if (errors.Any())
            {
                var failMessage = FormatSemanticErrors(startLine, programText, errors.ToList());
                Assert.Fail(failMessage);
            }
        }

        private string FormatSemanticErrors(int programLine, string programText, IList<Event> errorMessages)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendLine("Following errors was reported:");
            for (int i = 0; i < errorMessages.Count; i++)
            {
                var err = errorMessages[i];
                buffer.Append(i + 1);
                buffer.Append(". ");
                buffer.Append(err.Message);
                buffer.Append(" [src: ");
                buffer.Append(err.SourcePosition);
                buffer.Append("]");
                buffer.AppendLine();
            }

            buffer.Append("The program in question can be found in ");
            buffer.Append(SemanticSuccessfullTestCaseFactory.FileName);
            buffer.Append(" line ");
            buffer.Append(programLine);
            buffer.AppendLine(":");
            buffer.AppendLine(programText);

            return buffer.ToString();
        }

        private IList<Event> RunSemanticAnalysis(RootNode root)
        {
            foreach (var func in BuiltInFuncDeclNode.GetBuiltInFunctions())
                root.FuncDecls.Add(func);

            var reporter = new EventReporter();
            var scopeChecker = new ScopeCheckVisitor(reporter);
            var typeChecker = new TypeCheckVisitor(reporter);
            var breakChecker = new BreakCheckVisitor(reporter);
            var returnChecker = new ReturnCheckVisitor(reporter);
            root.Accept(scopeChecker, new VisitorArgs());
            root.Accept(typeChecker, new VisitorArgs());
            root.Accept(breakChecker, new VisitorArgs());
            root.Accept(returnChecker, new VisitorArgs());
            return reporter.GetEvents().Where(e => e.Kind == EventKind.Error).ToList();
        }

        private RootNode Parse(int startLine, string programText)
        {
            var parser = new Parser(Utils.GetGrammarTablePath());
            var result = parser.Parse(new StringReader(programText));
            if (result.Errors.Any())
            {
                var errorMessages = result.Errors.Select(e => e.GetErrorMessage()).ToArray();

                Assert.Fail(
                    $"Parser error for program at line {startLine}: \n\r \n\r{string.Join(", ", errorMessages)} \n\r{programText}");
            }

            return result.Root;
        }
    }
}
