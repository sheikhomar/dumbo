using System.IO;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public interface IParser
    {
        ParserResult Parse(TextReader reader);
    }
}
