using System.Collections;
using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;

namespace dumbo.Compiler.AST
{
    public abstract class BaseListNode<TBaseType> : BaseNode, IEnumerable<TBaseType> where TBaseType : BaseNode
    {
        private readonly IList<TBaseType> _internalList;

        protected BaseListNode()
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
        
        /// <summary>
        /// Find descendant nodes of a specific type T.
        /// </summary>
        /// <typeparam name="T">the type to look for</typeparam>
        /// <returns></returns>
        public IEnumerable<T> FindDescendants<T>() where T : class
        {
            var list = new List<T>();
            foreach (var item in _internalList)
            {
                T tItem = item as T;
                if (tItem != null)
                    list.Add(tItem);

                IHaveBlocks blocksItem = item as IHaveBlocks;
                if (blocksItem != null)
                {
                    foreach (var block in blocksItem.GetBlocks())
                    {
                        list.AddRange(block.FindDescendants<T>());
                    }
                }
            }

            return list;
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            if (_internalList.Count == 0)
                return;

            _internalList[0].PrettyPrint(prettyPrinter);

            if (_internalList.Count == 1)
                return;

            for (int i = 1; i < _internalList.Count; i++)
            {
                prettyPrinter.Append(", ");
                _internalList[i].PrettyPrint(prettyPrinter);
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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            foreach (var item in _internalList)
            {
                item.CCAnalyse(analyser);
            }
        }
    }
}