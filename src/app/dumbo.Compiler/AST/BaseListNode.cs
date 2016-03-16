using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class BaseListNode<TBaseType> : BaseNode
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
    }
}