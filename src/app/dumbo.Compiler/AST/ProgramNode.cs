using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : BaseNode, IHaveBlocks
    {
        public ProgramNode(StmtBlockNode body, SourcePosition srcPos)
        {
            Body = body;
            SourcePosition = srcPos;
        }

        public StmtBlockNode Body { get; }
        
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            Body.CCAnalyse(analyser);

            var returnStmts = Body.FindDescendants<ReturnStmtNode>();
            foreach (var returnStmt in returnStmts)
            {
                analyser.ErrorReporter.AddError(
                    new CCError($"Program cannot return values.", returnStmt.Line, returnStmt.Column));
            }

            Body.CheckBreaksAreNotUsed(analyser);
        }
        
        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }

}