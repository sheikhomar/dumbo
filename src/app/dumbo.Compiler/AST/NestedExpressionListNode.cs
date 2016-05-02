using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class NestedExpressionListNode : IEnumerable<object>
    {
        private readonly List<object> _internalList;

        public int Count => _internalList.Count;

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

        public IEnumerator<object> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            
            foreach (var i in _internalList)
            {
                if (i is ExpressionListNode)
                {
                    buffer.Append("[");
                    foreach (var exr in (i as ExpressionListNode))
                    {
                        var e = exr as LiteralValueNode;
                        buffer.Append(e.Value);
                        buffer.Append(", ");
                    }
                    buffer.Append("], ");
                }
                else
                {
                    buffer.Append("( ");
                    buffer.AppendLine(i.ToString());
                    buffer.Append("), ");
                }
            }

            return buffer.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CleanUp()
        {
            for (int i = 0; i < _internalList.Count; i++)
            {
                var list = _internalList[i] as NestedExpressionListNode;
                if (list?.Count == 0)
                {
                    _internalList.RemoveAt(i);
                }
            }
        }
    }
}