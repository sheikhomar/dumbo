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
    }
}