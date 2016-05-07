using System.Collections;
using System.Collections.Generic;
using System.IO;
using dumbo.Tests.SemanticAnalysis;
using NUnit.Framework;

namespace dumbo.Tests.SyntaxAnalysis
{
    public class TestProgramFactory
    {
        private TestProgramFactory() { }

        public static IEnumerable TestPrograms => BuildTestCases(nameof(TestPrograms));

        private static IEnumerable<TestCaseData> BuildTestCases(string fileName)
        {
            var dir = Path.GetDirectoryName(typeof(SemanticTests).Assembly.Location);
            var path = Path.Combine(dir, "SyntaxAnalysis", fileName + ".txt");

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
