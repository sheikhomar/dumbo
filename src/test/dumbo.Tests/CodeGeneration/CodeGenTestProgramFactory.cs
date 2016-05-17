using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace dumbo.Tests.CodeGeneration
{
    public class CodeGenTestProgramFactory
    {
        private CodeGenTestProgramFactory()
        {
        }

        public static IEnumerable TestPrograms => BuildTestCases(nameof(TestPrograms));

        private static IEnumerable<TestCaseData> BuildTestCases(string fileName)
        {
            var dir = Path.GetDirectoryName(typeof(CodeGenTestProgramFactory).Assembly.Location);
            var path = Path.Combine(dir, "CodeGeneration", fileName + ".txt");

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
