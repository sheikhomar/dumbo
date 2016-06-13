namespace dumbo.Compiler.MipsCode
{
    public class Register
    {
        public string Name { get; }

        public Register(string name)
        {
            Name = name;
        }

        public VariableLocation AssignedVariable { get; set; }
    }
}