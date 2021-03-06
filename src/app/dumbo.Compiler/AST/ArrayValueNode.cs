using System;
using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ArrayValueNode : ValueNode
    {
        public ArrayValueNode(ArrayTypeNode type, SourcePosition sourcePosition) : base(type)
        {
            ArrayType = type;
            Values = new NestedExpressionListNode();
            _currentNestListNode = Values;
            SourcePosition = sourcePosition;
        }

        public ArrayTypeNode ArrayType { get; }
        public NestedExpressionListNode Values { get; }

        private readonly Stack<NestedExpressionListNode> _listHistory = new Stack<NestedExpressionListNode>();
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
            _listHistory.Push(_currentNestListNode);
            _currentNestListNode = tempList;
        }

        public void CloseLevel()
        {
            if (_currentExpressionListNode == null)
            {
                _currentNestListNode = _listHistory.Pop();
            }
            else
            {
                _currentNestListNode = _listHistory.Pop();
                _currentNestListNode.CleanUp();
                _currentNestListNode.Add(_currentExpressionListNode);
            }
            _currentExpressionListNode = null;
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}