using System.Net.Mail;

namespace dumbo.Compiler.AST
{
    public class IfElseIfStmtNode : IfElseStmtNode
    {
        public IfElseIfStmtNode(ExpressionNode predicate, StmtBlockNode body, ElseIfStmtNode elseIfBody, StmtBlockNode @else)
            : base(predicate, body, @else)
        {
            ElseIfBody = elseIfBody;
        }

        public ElseIfStmtNode ElseIfBody { get; }
    }

    public class IfElseStmtNode : IfStmtNode
    {
        public IfElseStmtNode(ExpressionNode predicate, StmtBlockNode body, StmtBlockNode @else)
            : base(predicate, body)
        {
            Else = @else;
        }

        public StmtBlockNode Else { get; }
    }
    
    public class IfStmtNode : StmtNode
    {
        public IfStmtNode(ExpressionNode predicate, StmtBlockNode body)
        {
            Predicate = predicate;
            Body = body;
        }

        public ExpressionNode Predicate { get; }
        public StmtBlockNode Body { get;  }
    }
}