namespace dumbo.Compiler.Interpreter
{
    public class NumberValue : Value
    {
        public NumberValue(double number)
        {
            Number = number;
        }

        public double Number { get; }
    }
}