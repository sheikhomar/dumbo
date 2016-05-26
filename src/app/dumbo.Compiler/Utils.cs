using System.IO;

namespace dumbo.Compiler
{
    public class Utils
    {
        public static string GetGrammarTablePath()
        {
            return Path.Combine(GetBasePath(), "HappyZ-Grammar.egt");
        }

        public static string GetBasePath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
