using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ArrayValueNode : ValueNode
    {
        int level = 0;
        public ArrayValueNode(ArrayTypeNode type) : base(type)
        {
            ArrayType = type;
            Values = new NestedExpressionListNode();
            
        }

        public ArrayTypeNode ArrayType { get; }

        ///public ExpressionListNode Values { get; }
        public NestedExpressionListNode Values { get; }

        Stack<ExpressionListNode> _stack = new Stack<ExpressionListNode>();
        Stack<NestedExpressionListNode> _stack2 = new Stack<NestedExpressionListNode>();
        ExpressionListNode currentExpressionListNode = null;
         
        
        public void AddExpression(ExpressionNode expr)
        {
            _stack.Peek().Add(expr);
        }

        public void CreateLevel()
        {
            _stack.Push(new ExpressionListNode());
        }

        public void CloseLevel()
        {
            

            var listNode = _stack.Pop();
            var nestedListNode = new NestedExpressionListNode();
            nestedListNode.Add(listNode);
            Values.Add(nestedListNode);
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}