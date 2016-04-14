namespace dumbo.Compiler.Interpreter
{
    public class BooleanValue : Value
    {
        public BooleanValue(bool boolean)
        {
            Boolean = boolean;
        }

        public bool Boolean { get; set; }
    }
}