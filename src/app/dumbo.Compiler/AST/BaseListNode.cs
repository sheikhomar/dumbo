using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dumbo.Compiler.AST
{
    public abstract class BaseListNode<TBaseType> : BaseNode, IEnumerable<TBaseType> where TBaseType : BaseNode
    {
        private readonly IList<TBaseType> _internalList;

        public BaseListNode()
        {
            _internalList = new List<TBaseType>();
        }

        public void Add(TBaseType node)
        {
            _internalList.Add(node);
        }

        public TBaseType this[int index] => _internalList[index];

        public int Count => _internalList.Count;

        public T GetAs<T>(int index) where T : class, TBaseType
        {
            return _internalList[index] as T;
        }

        public IList<T> GetAllAs<T>() where T : class, TBaseType
        {
            return _internalList as List<T>;
        }

        public override void PrettyPrint(StringBuilder StrBuilder)
        {
            if (_internalList.Count == 0)
                return;

            _internalList[0].PrettyPrint(StrBuilder);

            if (_internalList.Count == 1)
                return;

            for (int i = 1; i < _internalList.Count; i++)
            {
                StrBuilder.Append(", ");
                _internalList[i].PrettyPrint(StrBuilder);
            }
        }

        public IEnumerator<TBaseType> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}