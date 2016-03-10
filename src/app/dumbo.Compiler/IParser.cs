using System.IO;

namespace dumbo.Compiler
{
    public interface IParser
    {
        AST Parse(TextReader reader);
    }
}