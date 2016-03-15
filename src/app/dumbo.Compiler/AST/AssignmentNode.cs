namespace dumbo.Compiler.AST
{
    public class AssignmentNode : StmtNode
    {
        public IdentifierNode Identifier;
        public ExpressionNode Expression;

    }
}