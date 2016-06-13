using System.IO;

namespace dumbo.Compiler
{
    public class Utils
    {
        public static string GetGrammarTablePath()
        {
            string grammarPath = System.AppDomain.CurrentDomain.BaseDirectory; 
            return Path.Combine(grammarPath, "HappyZ-Grammar.egt");
            //return Path.Combine(GetBasePath(), "HappyZ-Grammar.egt");
        }

        public static string GetBasePath()
        {
            string appDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string baseDir = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\..\\..\\"));
            return baseDir;
            //return System.AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
