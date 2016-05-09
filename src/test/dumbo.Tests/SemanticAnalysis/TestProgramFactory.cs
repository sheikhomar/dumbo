using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace dumbo.Tests.SemanticAnalysis
{
    public class TestProgramFactory
    {
        private TestProgramFactory()
        {
        }

        public static IEnumerable Assignment => BuildTestCases(nameof(Assignment));
        public static IEnumerable Break => BuildTestCases(nameof(Break));
        public static IEnumerable Constant => BuildTestCases(nameof(Constant));
        public static IEnumerable Continue => BuildTestCases(nameof(Continue));
        public static IEnumerable Declarations => BuildTestCases(nameof(Declarations));
        public static IEnumerable DeclarationsAndAssignment => BuildTestCases(nameof(DeclarationsAndAssignment));
        public static IEnumerable Expressions => BuildTestCases(nameof(Expressions));
        public static IEnumerable FunctionCall => BuildTestCases(nameof(FunctionCall));
        public static IEnumerable FunctionDeclaration => BuildTestCases(nameof(FunctionDeclaration));
        public static IEnumerable IfStatements => BuildTestCases(nameof(IfStatements));
        public static IEnumerable MiniPrograms => BuildTestCases(nameof(MiniPrograms));
        public static IEnumerable RepeatStatements => BuildTestCases(nameof(RepeatStatements));
        public static IEnumerable RepeatWhileStatements => BuildTestCases(nameof(RepeatWhileStatements));
        public static IEnumerable Return => BuildTestCases(nameof(Return));
        public static IEnumerable Scope => BuildTestCases(nameof(Scope));

        private static IEnumerable<TestCaseData> BuildTestCases(string fileName)
        {
            var dir = Path.GetDirectoryName(typeof(SemanticTests).Assembly.Location);
            var path = Path.Combine(dir, "SemanticAnalysis", "TestPrograms", fileName + ".txt");

            using (var reader = new TestProgramReader(path))
            {
                while (reader.ReadNext())
                {
                    var p = reader.CurrentTestProgram;
                    yield return new TestCaseData(fileName, p.Line, p.Text, p.ShouldFail);
                }
            }
        }
    }
}
