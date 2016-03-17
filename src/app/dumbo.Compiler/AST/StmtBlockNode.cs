using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseListNode<StmtNode>
    {
        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            var newList = GetAllAs<StmtNode>();

            if (newList.Count == 0)
                return;

            prettyPrinter.IndentIncrement(); //Inc. indentation
            foreach (var node in newList)
            {
                node.PrettyPrint(prettyPrinter);
            }
            prettyPrinter.IndentDecrement(); //Dec. indentation
        }

        public override void ScopeCheck(ISymbolTable table)
        {
            var newList = GetAllAs<StmtNode>();

            if (newList.Count == 0)
                return;

            foreach (var node in newList) 
            {
                node.ScopeCheck(table);
            }
        }
    }
}