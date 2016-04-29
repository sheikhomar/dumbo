using System.Collections;
using System.IO;
using NUnit.Framework;

namespace dumbo.Tests.SemanticAnalysis
{
    public class SemanticSuccessfullTestCaseFactory
    {
        public const string FileName = "SemanticTests.txt";


        private SemanticSuccessfullTestCaseFactory()
        {
        }

        public static IEnumerable TestCases
        {
            get
            {
                var dir = Path.GetDirectoryName(typeof(SemanticTests).Assembly.Location);
                var path = Path.Combine(dir, FileName);

                using (var reader = new ProgramReader(path))
                {
                    while (reader.ReadNext())
                    {
                        var p = reader.CurrentProgram;
                        yield return new TestCaseData(p.Line, p.Text);
                    }
                }

                //yield return new TestCaseData(1, "Program \n End Program");
            }
        }
    }
}
