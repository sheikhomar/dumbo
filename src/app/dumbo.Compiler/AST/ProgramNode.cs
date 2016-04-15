using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : FuncDeclNode, IHaveBlocks
    {
        public ProgramNode(StmtBlockNode body, SourcePosition srcPos) : 
            base("Program", body, srcPos)
        {
        }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}