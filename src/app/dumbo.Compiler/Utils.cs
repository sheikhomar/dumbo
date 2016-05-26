using System.IO;

namespace dumbo.Compiler
{
    public class Utils
    {
        public static string GetGrammarTablePath()
        {
            string grammarPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(grammarPath, "HappyZ-Grammar.egt");
            return path;
        }

        public static string GetProjectRootPath()
        {
            string appDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string baseDir = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\..\\..\\"));
            return baseDir;
        }
    }
}
