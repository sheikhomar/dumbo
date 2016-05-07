using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dumbo.Compiler;
using dumbo.Compiler.SyntaxAnalysis;
using NUnit.Framework;

namespace dumbo.Tests.SyntaxAnalysis
{
    [TestFixture]
    public class SyntaxTests : BaseTest
    {
        [Test, TestCaseSource(typeof(TestProgramFactory), nameof(TestProgramFactory.TestPrograms))]
        public void TestSyntax(string fileName, int startLine, string programText, bool shouldFail)
        {
            var result = Parse(programText);

            if (shouldFail && result.IsSuccess)
            {
                var msg = FormatShouldHaveFailedMessage(fileName, startLine, programText);
                Assert.Fail(msg);
            }
            else if (!shouldFail && !result.IsSuccess)
            {
                var msg = FormatShouldHaveSucceededMessage(fileName, startLine, programText, result.Errors.ToList());
                Assert.Fail(msg);
            }
        }

        protected string FormatShouldHaveSucceededMessage(string fileName, int programLine, string programText, IList<ParserError> errorMessages)
        {
            StringBuilder buffer = new StringBuilder();

            AppendErrors(errorMessages, buffer);
            AppendProgram(fileName, programLine, programText, buffer);

            return buffer.ToString();
        }

        protected string FormatShouldHaveFailedMessage(string fileName, int programLine, string programText)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendLine("Program was expected to fail the test, but didn't fail.");
            AppendProgram(fileName, programLine, programText, buffer);

            return buffer.ToString();
        }

        protected static void AppendErrors(IList<ParserError> errorMessages, StringBuilder buffer)
        {
            if (errorMessages.Count > 0)
            {
                buffer.AppendLine("Following errors were reported:");
                for (int i = 0; i < errorMessages.Count; i++)
                {
                    var err = errorMessages[i];
                    buffer.Append(i + 1);
                    buffer.Append(". ");
                    buffer.AppendLine(err.GetErrorMessage());
                }
            }
        }
    }
}