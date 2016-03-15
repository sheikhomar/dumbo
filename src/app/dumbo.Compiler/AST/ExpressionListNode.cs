using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ExpressionListNode : BaseNode
    {
        public ExpressionListNode(IList<ExpressionNode> expressions)
        {
            Expressions = expressions;
        }

        public IList<ExpressionNode> Expressions { get; }
    }
}