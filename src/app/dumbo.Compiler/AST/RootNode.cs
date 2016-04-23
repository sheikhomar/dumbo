namespace dumbo.Compiler.AST
{
    public class RootNode : BaseNode
    {
        public ProgramNode Program { get; set; }
        public FuncDeclListNode FuncDecls { get; }

        public RootNode(ProgramNode program)
        {
            Program = program;
            FuncDecls = new FuncDeclListNode();
        }
        
        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}