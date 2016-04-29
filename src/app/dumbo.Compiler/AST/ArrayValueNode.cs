using System;
using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ArrayValueNode : ValueNode
    {
        public ArrayValueNode(ArrayTypeNode type) : base(type)
        {
            ArrayType = type;
            Values = new NestedExpressionListNode();
            _currentNestListNode = Values;
        }

        public ArrayTypeNode ArrayType { get; }

        Stack<NestedExpressionListNode> ListHistory = new Stack<NestedExpressionListNode>();

        public NestedExpressionListNode Values { get; }

        private ExpressionListNode _currentExpressionListNode;
        private NestedExpressionListNode _currentNestListNode;
         
        public void AddExpression(ExpressionNode expr)
        {
            if (_currentExpressionListNode == null)
            {
                _currentExpressionListNode = new ExpressionListNode();
            }
            _currentExpressionListNode.Add(expr);
        }

        public void CreateLevel()
        {
            var tempList = new NestedExpressionListNode();
            _currentNestListNode.Add(tempList);
            ListHistory.Push(_currentNestListNode);
            _currentNestListNode = tempList;
        }

        public void CloseLevel()
        {
            if (_currentExpressionListNode == null)
            {
                _currentNestListNode = ListHistory.Pop();
            }
            else
            {
                _currentNestListNode.Add(_currentExpressionListNode);
                _currentNestListNode = ListHistory.Pop();
            }
            _currentExpressionListNode = null;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}