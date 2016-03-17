using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class IdentifierNode : ExpressionNode
    {
        public IdentifierNode(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Name);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            var hans = analyser.RetrieveSymbol(Name); //adde exception? otherwise its null.

            //Check if null, if so, then error message...

            
        }
    }
}