﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.Interpreter
{
    class ArrayValue : Value
    {
        public PrimitiveType Type { get; }
        public List<int> Sizes { get; }
        private List<Value> InternalArray { get; }
        public Value this[int i]
        {
            get { return InternalArray[i]; }
            set { InternalArray[i] = value; }
        }

        public ArrayValue(PrimitiveType type, List<int> sizes)
        {
            Type = type;
            Sizes = sizes;
            InternalArray = new List<Value>();
            InitializeArray();
        }

        private void InitializeArray()
        {
            if (Sizes.Count == 1)
            {
                for (int i = 0; i < Sizes[0]; i++)
                {
                    switch (Type)
                    {
                        case PrimitiveType.Number:
                            InternalArray.Add(new NumberValue(0));
                            break;
                        case PrimitiveType.Text:
                            InternalArray.Add(new TextValue(""));
                            break;
                        case PrimitiveType.Boolean:
                            InternalArray.Add(new BooleanValue(false));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                for (int i = 0; i < Sizes[0]; i++)
                {
                    InternalArray.Add(new ArrayValue(Type, Sizes.Skip(1).ToList()));
                }
            }
        }

        public Value GetValue(List<int> indices)
        {
            return GetValue(this, indices);
        }

        private Value GetValue(ArrayValue array, List<int> indices)
        {
            if (indices.Count == 1)
            {
                return array[indices[0]];
            }
            else
            {
                var newArray = array[indices[0]] as ArrayValue;
                var value = GetValue(newArray, indices.Skip(1).ToList());
                return value;
            }
        }

        public void SetValue(List<int> indices, Value value)
        {
            SetValue(this, indices, value);
        }

        private void SetValue(ArrayValue array, List<int> indices, Value value)
        {
            if (indices.Count == 1)
            {
                array[indices[0]] = value;
            }
            else
            {
                var newArray = array[indices[0]] as ArrayValue;
                SetValue(newArray, indices.Skip(1).ToList(), value);
            }
        }
    }
}
