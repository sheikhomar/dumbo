namespace dumbo.Compiler.AST
{
    public class BuiltFuncDeclNode : FuncDeclNode
    {
        public BuiltFunction Type { get; }

        public BuiltFuncDeclNode(BuiltFunction type) :
            base(type.ToString(), new StmtBlockNode(), new SourcePosition(0,0,0,0))
        {
            Type = type;
        }
    }
}