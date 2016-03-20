using System;
using System.Collections.Generic;
using System.Diagnostics;
using dumbo.Compiler.SyntaxAnalysis;
using GOLD;

namespace dumbo.Compiler.AST.Builder
{
    public class ConcreteTreeNode : IConcreteTreeNode
    {
        private readonly Dictionary<string, IList<Token>> _tokensTable = new Dictionary<string, IList<Token>>();
        private readonly IList<Token> _tokens = new List<Token>();
        private int _childCount;

        public string Name { get; private set; }

        public string Data { get; private set; }

        public int LineNumber { get; }

        public int Column { get; }

        public int ChildCount => _childCount;
        
        public ConcreteTreeNode(Reduction reduction)
        {
            Initialise(reduction);
        }

        public ConcreteTreeNode(Token token)
        {
            Name = token.Parent.Name();
            Data = string.Empty;
            var tokenData = token.Data as TokenData;
            if (tokenData != null)
            {
                Data = tokenData.ExtraData as string;
                LineNumber = tokenData.LineNumber;
                Column = tokenData.Column;
            }
            else
            {
                Initialise(token.Data as Reduction);
            }
        }

        private void Initialise(Reduction reduction)
        {
            if (reduction == null)
                throw new ArgumentNullException(nameof(reduction));

            Name = reduction.Parent.Head().Name();
            Data = string.Empty;
            _childCount = reduction.Count();

            for (int i = 0; i < _childCount; i++)
            {
                Token token = reduction[i];
                string name = token.Parent.Name();
                IList<Token> innerList;

                if (_tokensTable.ContainsKey(name))
                {
                    innerList = _tokensTable[name];
                }
                else
                {
                    innerList = new List<Token>();
                    _tokensTable.Add(name, innerList);
                }

                innerList.Add(token);
                _tokens.Add(token);
            }
        }

        public IConcreteTreeNode TryFindFirstChild(string childName)
        {
            if (_tokensTable.ContainsKey(childName))
                return new ConcreteTreeNode(_tokensTable[childName][0]);

            return null;
        }

        public IConcreteTreeNode GetFirstChild(string childName)
        {
            var child = TryFindFirstChild(childName);

            if (child == null)
                throw new ExceptedTokensNotFoundException(childName);

            return child;
        }

        public bool HasChild(string name)
        {
            return _tokensTable.ContainsKey(name);
        }

        public IConcreteTreeNode this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException($"Invalid index value detected: {index}.");

                if (index >= _childCount)
                    throw new ArgumentOutOfRangeException(
                        $"Invalid index value detected: {index}. This node has only {_childCount} children.");

                return new ConcreteTreeNode(_tokens[index]);
            }
        }

        
    }
}