﻿namespace dumbo.Compiler.AST
{
    public class ActualParamListNode : BaseListNode<ExpressionNode>
    {
        public override T Accept<T,K>(IVisitor<T,K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}