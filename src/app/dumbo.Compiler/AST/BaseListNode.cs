﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dumbo.Compiler.CCAnalysis;

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

        public IEnumerable<T> FindChildren<T>() where T : class, TBaseType
        {
            return _internalList.Where(r => r is T).Cast<T>();
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