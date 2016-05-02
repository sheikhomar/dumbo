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

        public int Count()
        {
            return _internalList.Count;
        }

        public object this[int i]
        {
            get { return _internalList[i]; }
        }

        public void Add(NestedExpressionListNode listNode)
        {
            _internalList.Add(listNode);
        }

        public void Add(ExpressionListNode listNode)
        {
            _internalList.Add(listNode);
        }

        public bool Delete(NestedExpressionListNode listNode)
        {
            var hashCode = listNode.GetHashCode();

            for (int i = 0; i < listNode.Count(); i++)
            {
                //if (listNode.Access(i).GetHashCode() == hashCode)
                //{
                //    _internalList.Remove(i);
                //    return true;
                //}
            }
            
            foreach (var list in _internalList)
            {
                var deleteBool = Delete(list as NestedExpressionListNode);
                if (deleteBool)
                {
                    return true;
                }
            }

            return false;
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
                    buffer.Append("]");
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