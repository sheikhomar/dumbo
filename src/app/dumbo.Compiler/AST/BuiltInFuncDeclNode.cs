namespace dumbo.Compiler.AST
{
    public class BuiltInFuncDeclNode : FuncDeclNode
    {
        public BuiltInFunction Type { get; }

        public BuiltInFuncDeclNode(BuiltInFunction type) :
            base(type.ToString(), new StmtBlockNode(), new SourcePosition(0, 0, 0, 0))
        {
            Type = type;
        }

        public override bool IsBuiltIn => true;

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}