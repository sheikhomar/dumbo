using System.Text;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public class FuncDeclListNode : BaseListNode<FuncDeclNode>
    {
        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            foreach (var node in this)
            {
                node.PrettyPrint(prettyPrinter);
            }
        }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}