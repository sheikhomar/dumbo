namespace dumbo.Compiler.AST
{
    public class FormalParamNode : BaseNode
    {
        public FormalParamNode(string name, HappyType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public HappyType Type { get; }
    }
}