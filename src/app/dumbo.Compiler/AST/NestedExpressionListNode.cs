using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class NestedExpressionListNode
    {
        private readonly List<object> _internalList;

        public NestedExpressionListNode()
        {
            _internalList = new List<object>();
        }

        public void Add(NestedExpressionListNode listNode)
        {
            _internalList.Add(listNode);
        }

        public void Add(ExpressionListNode listNode)
        {
            _internalList.Add(listNode);
        }
    }
}