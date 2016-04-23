using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class FuncDeclNode : BaseNode, IHaveBlocks
    {
        public FuncDeclNode(string name, StmtBlockNode body, SourcePosition sourcePosition)
        {
            Name = name;
            Body = body;
            ReturnTypes = new List<TypeNode>();
            Parameters = new FormalParamListNode();
            SourcePosition = sourcePosition;
        }

        public FormalParamListNode Parameters { get; }
        public IList<TypeNode> ReturnTypes { get; }
        public string Name { get; set; }
        public StmtBlockNode Body { get; }
        public virtual bool IsBuiltIn => false;

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