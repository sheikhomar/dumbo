using System.Diagnostics.Eventing;

namespace dumbo.Compiler.Interpreter
{
    public interface IInteractiveShell
    {
        void Write(string writeParameter);
        NumberValue ReadNumber();
        TextValue ReadText();
    }
}