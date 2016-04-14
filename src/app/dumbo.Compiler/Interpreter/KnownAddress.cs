namespace dumbo.Compiler.Interpreter
{
    public class KnownAddress : Value
    {
        public KnownAddress(int address)
        {
            Address = address;
        }

        public int Address { get; }
    }
}
