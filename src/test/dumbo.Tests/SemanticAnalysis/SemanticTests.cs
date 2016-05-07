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
    public class SemanticTests : BaseTest
    {
        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Assignment))]
        public void TestAssignments(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Break))]
        public void TestBreak(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Continue))]
        public void TestContinue(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Declarations))]
        public void TestDeclarations(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.DeclarationsAndAssignment))]
        public void TestDeclarationsAndAssignment(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Expressions))]
        public void TestExpressions(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.FunctionCall))]
        public void TestFunctionCall(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.FunctionDeclaration))]
        public void TestFunctionDeclaration(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.IfStatements))]
        public void TestIfStatements(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.MiniPrograms))]
        public void TestMiniPrograms(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.RepeatStatements))]
        public void TestRepeatStatements(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.RepeatWhileStatements))]
        public void TestRepeatWhileStatements(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Return))]
        public void TestReturn(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.Scope))]
        public void TestScope(string fileName, int startLine, string programText, bool shouldFail)
        {
            TestProgram(fileName, startLine, programText, shouldFail);
        }

        private void TestProgram(string fileName, int startLine, string programText, bool shouldFail)
        {
            var rootNode = GetRootNode(startLine, programText);
            var errors = RunSemanticAnalysis(rootNode);

            if (shouldFail)
            {
                if (errors.Count != 1)
                {
                    var failMessage = FormatShouldFailMessage(fileName, startLine, programText, errors);
                    Assert.Fail(failMessage);
                }
            }
            else
            {
                if (errors.Any())
                {
                    var failMessage = FormatShouldSucceedMessage(fileName, startLine, programText, errors.ToList());
                    Assert.Fail(failMessage);
                }
            }
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

        private RootNode GetRootNode(int startLine, string programText)
        {
            var result = Parse(programText);
            if (result.Errors.Any())
            {
                var errorMessages = result.Errors.Select(e => e.GetErrorMessage()).ToArray();

                Assert.Fail(
                    $"Parser error for program at line {startLine}: \n\r \n\r{string.Join(", ", errorMessages)} \n\r{programText}");
            }

            return result.Root;
        }


        protected string FormatShouldFailMessage(string fileName, int programLine, string programText, IList<Event> errorMessages)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendLine("Program was expected to fail the test, but didn't fail.");
            AppendErrors(errorMessages, buffer);
            AppendProgram(fileName, programLine, programText, buffer);

            return buffer.ToString();
        }

        protected string FormatShouldSucceedMessage(string fileName, int programLine, string programText, IList<Event> errorMessages)
        {
            StringBuilder buffer = new StringBuilder();

            AppendErrors(errorMessages, buffer);
            AppendProgram(fileName, programLine, programText, buffer);

            return buffer.ToString();
        }

        protected static void AppendErrors(IList<Event> errorMessages, StringBuilder buffer)
        {
            if (errorMessages.Count > 0)
            {
                buffer.AppendLine("Following errors were reported:");
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
            }
        }
    }
}
