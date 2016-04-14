namespace dumbo.Compiler.AST
{
    public class FuncCallExprNode : ExpressionNode
    {
        public FuncCallExprNode(string funcName, SourcePosition sourcePosition)
        {
            FuncName = funcName;
            SourcePosition = sourcePosition;
            Parameters = new ActualParamListNode();
        }

        public string FuncName { get; }
        public ActualParamListNode Parameters { get; }
        public FuncDeclNode DeclarationNode { get; set; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}