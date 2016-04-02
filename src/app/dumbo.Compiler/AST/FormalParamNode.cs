using dumbo.Compiler.CCAnalysis;

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
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}