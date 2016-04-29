using System.Collections.Generic;
using System.Text;

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

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            
            foreach (var i in _internalList)
            {
                if (i is ExpressionListNode)
                {
                    foreach (var exr in (i as ExpressionListNode))
                    {
                        var e = exr as LiteralValueNode;
                        buffer.Append(e.Value);
                        buffer.Append(", ");
                    }
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
    }
}