using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class FormalParamNode : BaseNode, IVariableDeclNode
    {
        public FormalParamNode(string name, HappyType type, SourcePosition sourcePosition)
        {
            Name = name;
            Type = type;
            SourcePosition = sourcePosition;
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