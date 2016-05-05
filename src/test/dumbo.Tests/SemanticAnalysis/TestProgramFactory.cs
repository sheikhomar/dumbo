using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace dumbo.Tests.SemanticAnalysis
{
    public class TestProgramFactory
    {
        public const string FileName = "SemanticTests";
        
        private TestProgramFactory()
        {
        }

        public static IEnumerable TestCases => BuildTestCases(FileName);

        public static IEnumerable Declarations => BuildTestCases(nameof(Declarations));

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
