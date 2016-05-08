using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace dumbo.Compiler.AST
{
    public class ArrayTypeNode : TypeNode, IEquatable<ArrayTypeNode>
    {
        private readonly IList<int?> _parsedDimensions;

        public PrimitiveTypeNode Type { get; set; }
        public ExpressionListNode Sizes { get; set; }

        public ArrayTypeNode(PrimitiveTypeNode type, ExpressionListNode sizes, SourcePosition srcPos)
        {
            Type = type;
            Sizes = sizes;
            SourcePosition = srcPos;
            _parsedDimensions = ParseDimensions();
        }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ArrayTypeNode);
        }
        
        public bool Equals(ArrayTypeNode other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!Type.Equals(other.Type))
                return false;

            if (Sizes.Count != other.Sizes.Count)
                return false;

            for (int i = 0; i < Sizes.Count; i++)
            {
                var mySize = _parsedDimensions[i];
                var otherSize = other._parsedDimensions[i];
                
                if (mySize == null || otherSize == null)
                {
                    return true;
                }

                if (mySize != otherSize)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var dim = _parsedDimensions.Select(e => e.HasValue ? e.Value.ToString() : "x");

            builder.Append("Array[");
            builder.Append(string.Join(", ", dim));
            builder.Append("] of ");
            builder.Append(Type);

            return builder.ToString();
        }

        private IList<int?> ParseDimensions()
        {
            IList<int?> retList = new List<int?>();
            foreach (var size in Sizes)
            {
                var sizeLiteral = size as LiteralValueNode;
                if (sizeLiteral != null)
                {
                    double parsedVal;
                    if (double.TryParse(sizeLiteral.Value, out parsedVal))
                    {
                        var sizeAsInt = (int) Math.Floor(parsedVal);
                        retList.Add(sizeAsInt);
                    }
                    else
                    {
                        retList.Add(null);
                    }
                }
                else
                {
                    retList.Add(null);
                }
            }
            return retList;
        }
    }
}