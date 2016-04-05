using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseListNode<StmtNode>
    {
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            analyser.SymbolTable.OpenScope();
            base.CCAnalyse(analyser);
            analyser.SymbolTable.CloseScope();
        }

        public void CheckBreaksAreNotUsed(ICCAnalyser analyser)
        {
            var breakStmts = FindChildren<BreakStmtNode>();
            foreach (var breakStmt in breakStmts)
            {
                analyser.ErrorReporter.AddError(
                    new CCError($"Wrong use of statemet. 'Break' can only be used inside 'Repeat'.", breakStmt.Line, breakStmt.Column));
            }
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}