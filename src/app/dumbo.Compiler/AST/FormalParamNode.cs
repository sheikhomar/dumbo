using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;

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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            // Do nothing
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Type.ToString() + " " + Name);
        }
    }
}