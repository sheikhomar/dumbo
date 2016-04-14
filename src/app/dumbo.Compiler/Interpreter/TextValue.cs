namespace dumbo.Compiler.Interpreter
{

    public class TextValue : Value
    {
        public TextValue(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}